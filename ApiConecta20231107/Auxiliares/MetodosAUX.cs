using API.DTOs;
using API.DTOs.OtrosSistemas;
using ApiConecta20231107.DTOs.OtrosSistemas;
using Data;
using Entidades;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Servicios;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;

namespace API.Auxiliares
{
    public class MetodosAUX: IMetodosAUX
    {
        private readonly IEmpleadosServicio _empleadosServicio;
        private readonly IHistorialEmpleadoServicio _historialEmpleadoServicio;
        private readonly IConfiguration configuration;

        public MetodosAUX(  IEmpleadosServicio empleadosServicio,
                            IHistorialEmpleadoServicio historialEmpleadoServicio,
                            IConfiguration configuration)
        {
            this._empleadosServicio = empleadosServicio;
            this._historialEmpleadoServicio = historialEmpleadoServicio;
            this.configuration = configuration;
        }



        




        public List<string[]> HistorialGenerico(string tabla, string filtroTabla, string filtroEmpleado)
        {

            var query = from t1 in _historialEmpleadoServicio.ObtenerLista()
                        join t2 in _empleadosServicio.ObtenerConsulta()
                        on t1.CreadoPor equals t2.Id.ToString()
                        where t1.EmpleadoId.ToString()==filtroEmpleado && t1.Identificador.Contains(filtroTabla)
                        orderby t1.FechaCreacion
                        select new string[] 
                        {  
                            t2.Nombre+" "+t2.ApellidoPaterno,
                            t1.NuevoCambio,
                            t1.FechaCreacion.Value.ToString("yyyy-MM-dd"),
                        };

            return query.ToList();
        }




        public string UploadFiles(List<IFormFile> files, int solicitudId, string _contexEmpleadoId)
        {
            // Iterar a través de los archivos y guardarlos en el servidor

            foreach (var file in files)
            {
                // Guardar el archivo en el directorio deseado

                if (file.Length > 0)
                {
                    var filePath = Path.GetTempFileName();
                    var filebase64 = ConvertFileToBase64(file);
                    Guid Id = Guid.NewGuid();
                    DocumentoDTO documento = new DocumentoDTO()
                    {
                        NameFile = DateTime.Now.ToString("yyyymmddhhmmss_") + file.FileName,
                        DocumentBase64 = filebase64,
                        Id = Id,
                        systemName = "Conecta",
                        DocumentType = "Conecta",
                        UserId = Guid.NewGuid(),
                        IdentificationId = Guid.NewGuid(),
                        Folio = "1",
                        Status = 1,
                        Origin = "Conecta"
                    };

                    Guid idExterno = SendDocumentFileServer(documento);

                    //DocumentosSolicitudes doc = new DocumentosSolicitudes()
                    //{
                    //    NameFile = DateTime.Now.ToString("yyyymmddhhmmss_") + file.FileName,
                    //    PathFile = filePath,
                    //    SolicitudId = solicitudId,
                    //    idExterno = idExterno,

                    //};

                    //_documentosSolicitudesServicio.Crear(doc, _contexEmpleadoId);

                }
            }

            // Devolver una respuesta adecuada según tus necesidades

            return Constantes.RESPUESTA_OK;
        }

        public static string ConvertFileToBase64(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
                return s;
            }
        }

        /// <summary>
        /// Metodo para mandar documento servidor de documentos
        /// </summary>
        /// <param name="docModel"></param>
        /// <returns></returns>
        public Guid SendDocumentFileServer(DocumentoDTO docModel)
        {
            Guid result = Guid.Empty;

            var webApiUrl = configuration["API_DOCUMENT_createDocument"];//  "http://apidocumentv3.sisec.mx/api/document/createDocument";
            string inputJson = JsonConvert.SerializeObject(docModel);
            HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            HttpResponseMessage respons = client.PostAsync(webApiUrl, inputContent).Result;
            var result1 = respons.Content.ReadAsStringAsync().Result;
            if (respons.StatusCode == HttpStatusCode.OK)
            {
                result= JsonConvert.DeserializeObject<DocumentoDTO>(result1).documentId;
            };

            return result;
        }


        public async Task<GeneralDTO> ObtenerOtroSistema(string id,string url,string sistema)
        {
            GeneralDTO result = new GeneralDTO();
            string tockenExterno = GetTokenExternos();
            HttpClient apiClient = new HttpClient();
            apiClient.SetBearerToken(tockenExterno);
            var requestData = new { Id = id };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            url = url + "?Id=" + id;
            HttpResponseMessage respons;
            respons = apiClient.PostAsync(url, content).Result;
            if (respons.IsSuccessStatusCode)
            {
                var jsonResponse = respons.Content.ReadAsStringAsync().Result;
                result = MapRespuestaOtroSistema(jsonResponse, sistema);

            }
            else
            {
                var jsonResponseError = respons.Content.ReadAsStringAsync().Result;
                              
            }

            return result;

        }

      


        /// <summary>
        /// Metodo para mandar empleado a otro sistema, usa metodo post y put
        /// </summary>
        /// <param name="generalDTO"></param>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string MandarOtroSistema(GeneralDTO generalDTO,string url,string type = "post")
        {
            string tockenExterno = GetTokenExternos();
            string result = string.Empty;
            HttpClient apiClient = new HttpClient();
            apiClient.SetBearerToken(tockenExterno);
            //apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tockenExterno);
            var webApiUrl = url;
            string inputJson = JsonConvert.SerializeObject(generalDTO);
            HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");

            HttpResponseMessage respons;

            if(type=="post")
                respons = apiClient.PostAsync(webApiUrl, inputContent).Result;
            else
                respons = apiClient.PutAsync(webApiUrl, inputContent).Result;

            if (respons.IsSuccessStatusCode)
            {
                var jsonResponse = respons.Content.ReadAsStringAsync().Result;
                result = Constantes.RESPUESTA_OK;
            }
            else
            {
                var jsonResponseError = respons.Content.ReadAsStringAsync().Result;

            }

            return result;
        }






        public ResponseWebApiMulti PostWebAPiUsersMultiLogin<T>(T entity, string url)
        {
            return PostWebAPiUsersMultiLoginRESULT(entity, url).Result;
        }


        public  async Task<ResponseWebApiMulti> PostWebAPiUsersMultiLoginRESULT<T>(T entity, string url)
        {
            var response = new ResponseWebApiMulti();
            var result = string.Empty;
            var json = string.Empty;
            HttpResponseMessage responseApi = new HttpResponseMessage();
            try
            {
                //var clientId = "soc_identityapi_conecta";
                //var clientSecret = "5ce8d353-e9a9-4cbf-72de-1d253128873e";
                //var scope = "IdentityServerApi";
                var clientId = configuration["API_MULTILOGIN_identityapi"];
                var clientSecret = configuration["API_MULTILOGIN_clientSecret"];
                var scope = configuration["API_MULTILOGIN_scope"];

                var token = GetTokenMultilogin(clientId, clientSecret, scope);

                var apiClient = new HttpClient();
                apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                json = JsonConvert.SerializeObject(entity);
                StringContent input = new StringContent(json, Encoding.UTF8, "application/json");
                responseApi = await apiClient.PostAsync(url, input);

                result = await responseApi.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<ResponseWebApiMulti>(result);
            }
            catch (Exception e)
            {
                string error = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Metodo para obtener tocken de multilogin
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="clientScopes"></param>
        /// <returns></returns>
        public string GetTokenMultilogin(string clientId, string clientSecret, string clientScopes)
        {
            var token = string.Empty;
            var client = new HttpClient();
            //var authority = "https://pserverlogin.sisec.mx";
            var authority = configuration["API_MULTILOGIN_login"];

            var response = client.RequestTokenAsync(new TokenRequest
            {
                Address = $"{authority}/connect/token",
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Parameters =
                {
                    {"scope", clientScopes}
                }

            }).Result;
            if (!response.IsError)
                token = response.AccessToken;

            return token;
        }

        /// <summary>
        /// Metodo para obtener el tocken de autorizacion de apiexternos.sisec
        /// </summary>
        /// <returns></returns>
        public string GetTokenExternos()
        {
            var token = string.Empty;
            HttpClient apiClient = new HttpClient();
            try
            {
                var ussPass = new { username = configuration["API_EXTERNO_uss"], password = configuration["API_EXTERNO_pass"] };
                var inputJson = JsonConvert.SerializeObject(ussPass);
                HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
                HttpResponseMessage respons = apiClient.PostAsync(configuration["API_EXTERNO_token"], inputContent).Result;
                if (respons.IsSuccessStatusCode)
                {
                    // Leer el contenido como cadena JSON
                    string jsonResponse = respons.Content.ReadAsStringAsync().Result;

                    // Deserializar el JSON para obtener el token
                    var tokenObject = JsonConvert.DeserializeAnonymousType(jsonResponse, new { token = "" });
                    token = tokenObject.token;
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
            }

            return token;
        }


        public string MandarCorreoNuevoUsuario(string nombre, string usuario, string correo, string sistemas)
        {
            string result = string.Empty;

            //Obtiene correo
            string body = ObtenerCorreohtml();
            //Edita correo
            body = body.Replace("[*NOMBREEMPLEADO*]", nombre);
            body = body.Replace("[*USUARIO*]", usuario);
            body = body.Replace("[*PASSWORD*]", "n3wPa$$w0rdT3st");
            body = body.Replace("[*TEST*]", "*******************Correo enviado desde pruebas, por favor hacer caso omiso");

            string botones = string.Empty;
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_SISEC))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_SISEC);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_AGENTESOC))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_AGENTESOC);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_BROKERMASTER))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_BROKERMASTER);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_CAPTURA))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_CAPTURA);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_CREDIHIPO))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_CREDIHIPO);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_RESUELVEME))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_RESUELVEME);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_SICAFI))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_SICAFI);
            if (sistemas.Contains(Constantes.OTRO_SISTEMA_VALIDOC))
                botones = botones + ObtenerBotonSistema(Constantes.OTRO_SISTEMA_VALIDOC);

            body = body.Replace("[*BOTONES*]", botones);



            MandarCorreo("Usuario Creado", correo , body);

            return result;
        }

        public string MandarCorreoPass(string usuario, string correo, string nuevoPass)
        {
            string result = string.Empty;
            try
            {  
                string body = correoPassReestablecida;
                body = body.Replace("[*USUARIO*]", usuario);
                body = body.Replace("[*PASSWORD*]", nuevoPass);
                body = body.Replace("[*TEST*]", "*******************Correo enviado desde pruebas, por favor hacer caso omiso");
                string botones = ObtenerBotonSistema(Constantes.OTRO_SISTEMA_CONECTA);
                body = body.Replace("[*BOTONES*]", botones);
                MandarCorreo("Contraseña actualizada", correo, body);
                result = Constantes.RESPUESTA_OK;
            }
            catch
            {

            }
            return result;
        }



        public GeneralDTO MapRespuestaOtroSistema(string Json, string sistema)
        {
            GeneralDTO result = new GeneralDTO();

            if (string.IsNullOrEmpty(Json))
                return result;

            switch (sistema)
            {
                case Constantes.OTRO_SISTEMA_SISEC:
                    result = JsonConvert.DeserializeObject<SisecDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_AGENTESOC:
                    result = JsonConvert.DeserializeObject<AgenteSOCDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_BROKERMASTER:
                    result = JsonConvert.DeserializeObject<BrokersMasterDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_CAPTURA:
                    result = JsonConvert.DeserializeObject<CapturaDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_CREDIHIPO:
                    result = JsonConvert.DeserializeObject<CrediHipotecarioDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_RESUELVEME:
                    result = JsonConvert.DeserializeObject<ResuelvemeDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_SICAFI:
                    result = JsonConvert.DeserializeObject<SicafiDTO>(Json);
                    break;
                case Constantes.OTRO_SISTEMA_VALIDOC:
                    result = JsonConvert.DeserializeObject<ValidocDTO>(Json);
                    break;

            }
            return result;
        }

        public string ObtenerBotonSistema(string sistema)
        {
            string result = botonSistema;

            try
            {
                switch (sistema) 
                {
                    case Constantes.OTRO_SISTEMA_SISEC:
                        result = result.Replace("[*Sistema*]", "Sisec");
                        result = result.Replace("[*LINK*]", configuration["URL_SISEC"]);
                        break;
                    case Constantes.OTRO_SISTEMA_AGENTESOC:
                        result = result.Replace("[*Sistema*]", "Agente Soc");
                        result = result.Replace("[*LINK*]", configuration["URL_AGENTESOC"]);
                        break;
                    case Constantes.OTRO_SISTEMA_BROKERMASTER:
                        result = result.Replace("[*Sistema*]", "BrokersMaster");
                        result = result.Replace("[*LINK*]", configuration["URL_BROKERMASTER"]);
                        break;
                    case Constantes.OTRO_SISTEMA_CAPTURA:
                        result = result.Replace("[*Sistema*]", "Captura");
                        result = result.Replace("[*LINK*]", configuration["URL_CAPTURA"]);
                        break;
                    case Constantes.OTRO_SISTEMA_CREDIHIPO:
                        result = result.Replace("[*Sistema*]", "Credihipotecario");
                        result = result.Replace("[*LINK*]", configuration["URL_CREDIPOTECARIO"]);
                        break;
                    case Constantes.OTRO_SISTEMA_RESUELVEME:
                        result = result.Replace("[*Sistema*]", "Resuelveme");
                        result = result.Replace("[*LINK*]", configuration["URL_RESUELVEME"]);
                        break;
                    case Constantes.OTRO_SISTEMA_SICAFI:
                        result = result.Replace("[*Sistema*]", "Sicafi");
                        result = result.Replace("[*LINK*]", configuration["URL_SICAFI"]);
                        break;
                    case Constantes.OTRO_SISTEMA_VALIDOC:
                        result = result.Replace("[*Sistema*]", "Validoc");
                        result = result.Replace("[*LINK*]", configuration["URL_VALIDOC"]);
                        break;
                    case Constantes.OTRO_SISTEMA_CONECTA:
                        result = result.Replace("[*Sistema*]", "Conecta");
                        result = result.Replace("[*LINK*]", configuration["URL_CONECTA"]);
                        break;
                }
            }
            catch { };

            return result;
        }

        public string ObtenerCorreohtml()
        {
            string result = string.Empty;
            string rutaCompleta = Path.GetFullPath("Documentos/UsuarioContrasenia.html");
            try
            {
                string body = correo; //System.IO.File.ReadAllText(rutaCompleta);
                result = body;
            }
            catch (Exception ex)
            {
                result = correo;
            }

            return result;
        }

        public string MandarCorreo(string asunto, string destinatario, string cuerpo)
        {

            destinatario = "desarrollo@grupoapa.com.mx";
            

            string result = string.Empty;
            try
            {
                MailMessage mail = new MailMessage();
                var smtp = new SmtpClient();
                mail.From = new MailAddress("pruebasmail@socasesores.com");
                mail.IsBodyHtml = true;
                mail.Subject = asunto;
                mail.Body = cuerpo;
                mail.To.Add(destinatario);


                mail.CC.Add("brchavez@socasesores.com.mx");
                mail.CC.Add("jochavez@socasesores.com");


                var credentials = new NetworkCredential("sisec@sisec.mx", "XadKnN6bQqfyc1O4");
                smtp.Credentials = credentials;
                smtp.Port = 587;
                smtp.EnableSsl = false;
                smtp.Host = "smtp-relay.sendinblue.com";
                smtp.Send(mail);

                result = Constantes.RESPUESTA_OK;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }





        const string correo =
            @"<!doctype html>
<html>
<head>
    <meta name=""viewport"" content=""width=device-width"">
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
    <title></title>
    <style>
        @media only screen and (max-width: 620px) {
            table[class=body] h1 {
                font-size: 28px !important;
                margin-bottom: 10px !important;
            }

            table[class=body] p,
            table[class=body] ul,
            table[class=body] ol,
            table[class=body] td,
            table[class=body] span,
            table[class=body] a {
                font-size: 16px !important;
            }

            table[class=body] .wrapper,
            table[class=body] .article {
                padding: 10px !important;
            }

            table[class=body] .content {
                padding: 0 !important;
            }

            table[class=body] .container {
                padding: 0 !important;
                width: 100% !important;
            }

            table[class=body] .main {
                border-left-width: 0 !important;
                border-radius: 0 !important;
                border-right-width: 0 !important;
            }

            table[class=body] .btn table {
                width: 100% !important;
            }

            table[class=body] .btn a {
                width: 100% !important;
            }

            table[class=body] .img-responsive {
                height: auto !important;
                max-width: 100% !important;
                width: auto !important;
            }
        }

        @media all {
            .ExternalClass {
                width: 100%;
            }

                .ExternalClass,
                .ExternalClass p,
                .ExternalClass span,
                .ExternalClass font,
                .ExternalClass td,
                .ExternalClass div {
                    line-height: 100%;
                }

            .apple-link a {
                color: inherit !important;
                font-family: inherit !important;
                font-size: inherit !important;
                font-weight: inherit !important;
                line-height: inherit !important;
                text-decoration: none !important;
            }

            #MessageViewBody a {
                color: inherit;
                text-decoration: none;
                font-size: inherit;
                font-family: inherit;
                font-weight: inherit;
                line-height: inherit;
            }
            .box {
                display: flex !important;
                flex-wrap: wrap !important;
                gap: 10px !important;
                background-color: #1f9382 !important;
            }

                .box > * {
                    flex: 1 1 20% !important;
                }
            .btn-primary table td:hover {
                background-color: #34495e !important;
            }

            .btn-primary a:hover {
                background-color: #34495e !important;
                border-color: #34495e !important;
            }

            .grid-container {
                display: grid !important;
                grid-template-columns: 33% 33% 33%;
                background-color: #2196F3;
                padding: 10px;
            }

            .grid-item {
                background-color: rgba(255, 255, 255, 0.8);
                border: 1px solid rgba(0, 0, 0, 0.8);
                padding: 20px;
                font-size: 30px;
                text-align: center;
            }
        }
    </style>
</head>
<body class="""" style=""background-color: #f6f6f6; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""body"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background-color: #f6f6f6;"">
        <tr>
            <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
            <td class=""container"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; Margin: 0 auto; max-width: 580px; padding: 10px; width: 580px;"">
                <div class=""content"" style=""box-sizing: border-box; display: block; Margin: 0 auto; max-width: 580px; padding: 10px;"">

                    <table class=""main"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background: #ffffff; border-radius: 3px;"">

                        <tr>
                            <td class=""wrapper"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box; padding: 20px;"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;"">
                                    <tr>
                                        <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">
                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                Hola, <span style=""font-weight: bold; color: #1f9382; "">[*NOMBREEMPLEADO*].</span>
                                            </p>
                                            <p style=""font-family: sans-serif; font-size: 14px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                <span style=""font-weight: bold;"">
                                                    Bienvenido a
                                                </span>
                                                <br />
                                                <br />
                                                <a href=""https://conecta-389821.uc.r.appspot.com/auth/login"" target=""_blank"">
                                                    <img src=""https://mcusercontent.com/e4fbfdae0aba10ba8c7716bab/images/49f1a8ed-a1b2-c1c9-22f9-b6613bba03e4.png"" width=""230"">
                                                </a>
                                                <br />
                                                <br />
                                                <br />
                                                <br />
                                                Tu sistema RH, diseñado para facilitar y optimizar los procesos relacionados con la administración de tiempo y el personal dentro de tu organización.
                                            </p>
                                            <br />
                                            <br />
                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                Tu usuario es: <span style=""font-weight: bold; color: #1f9382; "">[*USUARIO*]</span>
                                                <br>
                                                Tu contraseña es: <span style=""font-weight: bold; color: #1f9382; "">[*PASSWORD*] </span>
                                            </p>
                                            <br>
                                            <br>

                                            <p style=""font-family: sans-serif; font-size: 14px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                Con los datos proporcionados, puedes ingresar a 
                                                <a style=""text-decoration:none; color: #1f9382;"" href=""https://conecta-389821.uc.r.appspot.com/auth/login"" target=""_blank"">
                                                    Conecta
                                                </a>
                                                <br>
                                                y a los siguientes sistemas:
                                            </p>
                                            <br>
                                            <div class=""box"">
                                                [*BOTONES*]
                                            </div>

                                                <p style=""font-family: sans-serif; font-size: 13px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                    *Te sugerimos realizar el cambio de contraseña en el módulo de perfil, que se encuentra en la parte superior derecha del navegador.
                                                </p>
                                                <br>
                                                <p style=""font-family: sans-serif; font-size: 13px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                    [*TEST*]
                                                </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
        </tr>
    </table>
</body>
</html>";

        const string botonSistema = @"  <div style=""background-color:#1f9382 !important;"">                                  
                                        <a href=""[*LINK*]"" 
                                            style=""color: #ffffff; cursor: pointer; text-decoration: none; font-size: 14px; background-color: #1f9382 !important; "">
                                             <div class=""btn btn-primary"" 
                                                style=""display: block; color: #ffffff; background-color: #1f9382 !important;
                                                        border: solid 1px #1f9382; border-radius: 5px; box-sizing: border-box;
                                                        font-weight: bold; margin: 0; padding: 12px 25px; "">
                                             Ir a [*Sistema*]
                                             </div>
                                         </a>
                                        </div>
                                     ";

        public const string correoPassReestablecida =
         @"<!doctype html>
            <html>
            <head>
                <meta name=""viewport"" content=""width=device-width"">
                <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
                <title></title>
                <style>
                    @media only screen and (max-width: 620px) {
                        table[class=body] h1 {
                            font-size: 28px !important;
                            margin-bottom: 10px !important;
                        }

                        table[class=body] p,
                        table[class=body] ul,
                        table[class=body] ol,
                        table[class=body] td,
                        table[class=body] span,
                        table[class=body] a {
                            font-size: 16px !important;
                        }

                        table[class=body] .wrapper,
                        table[class=body] .article {
                            padding: 10px !important;
                        }

                        table[class=body] .content {
                            padding: 0 !important;
                        }

                        table[class=body] .container {
                            padding: 0 !important;
                            width: 100% !important;
                        }

                        table[class=body] .main {
                            border-left-width: 0 !important;
                            border-radius: 0 !important;
                            border-right-width: 0 !important;
                        }

                        table[class=body] .btn table {
                            width: 100% !important;
                        }

                        table[class=body] .btn a {
                            width: 100% !important;
                        }

                        table[class=body] .img-responsive {
                            height: auto !important;
                            max-width: 100% !important;
                            width: auto !important;
                        }
                    }

                    @media all {
                        .ExternalClass {
                            width: 100%;
                        }

                            .ExternalClass,
                            .ExternalClass p,
                            .ExternalClass span,
                            .ExternalClass font,
                            .ExternalClass td,
                            .ExternalClass div {
                                line-height: 100%;
                            }

                        .apple-link a {
                            color: inherit !important;
                            font-family: inherit !important;
                            font-size: inherit !important;
                            font-weight: inherit !important;
                            line-height: inherit !important;
                            text-decoration: none !important;
                        }

                        #MessageViewBody a {
                            color: inherit;
                            text-decoration: none;
                            font-size: inherit;
                            font-family: inherit;
                            font-weight: inherit;
                            line-height: inherit;
                        }
                        .box {
                            display: flex !important;
                            flex-wrap: wrap !important;
                            gap: 10px !important;
                            background-color: #1f9382 !important;
                        }

                            .box > * {
                                flex: 1 1 20% !important;
                            }
                        .btn-primary table td:hover {
                            background-color: #34495e !important;
                        }

                        .btn-primary a:hover {
                            background-color: #34495e !important;
                            border-color: #34495e !important;
                        }

                        .grid-container {
                            display: grid !important;
                            grid-template-columns: 33% 33% 33%;
                            background-color: #2196F3;
                            padding: 10px;
                        }

                        .grid-item {
                            background-color: rgba(255, 255, 255, 0.8);
                            border: 1px solid rgba(0, 0, 0, 0.8);
                            padding: 20px;
                            font-size: 30px;
                            text-align: center;
                        }
                    }
                </style>
            </head>
            <body class="""" style=""background-color: #f6f6f6; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;"">
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""body"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background-color: #f6f6f6;"">
                    <tr>
                        <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
                        <td class=""container"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; Margin: 0 auto; max-width: 580px; padding: 10px; width: 580px;"">
                            <div class=""content"" style=""box-sizing: border-box; display: block; Margin: 0 auto; max-width: 580px; padding: 10px;"">

                                <table class=""main"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background: #ffffff; border-radius: 3px;"">

                                    <tr>
                                        <td class=""wrapper"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box; padding: 20px;"">
                                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;"">
                                                <tr>
                                                    <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">
                                                        <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                            Hola, hemos actualizado tu contraseña
                                                        </p>
                                                        <p style=""font-family: sans-serif; font-size: 14px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                            <br />
                                                            <br />
                                                            <img src=""https://mcusercontent.com/e4fbfdae0aba10ba8c7716bab/images/49f1a8ed-a1b2-c1c9-22f9-b6613bba03e4.png"" width=""230"">
                                                            <br />
                                                            <br />
                                                            <br />
                                                            <br />
                                                            Respondiendo a tu peticion hemos actualizado tu contraseña, recuerda que siempre estapos para apoyarte
                                                        </p>
                                                        <br />
                                                        <br />
                                                        <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                            Tu usuario es: <span style=""font-weight: bold; color: #1f9382; "">[*USUARIO*]</span>
                                                            <br>
                                                            Tu contraseña es: <span style=""font-weight: bold; color: #1f9382; "">[*PASSWORD*] </span>
                                                        </p>
                                                        <br>
                                                        <br>

                                                        <p style=""font-family: sans-serif; font-size: 14px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                            Con los datos proporcionados, puedes ingresar a Conecta
                                                        </p>
                                                        <br>
                                                        <div class=""box"">
                                                            [*BOTONES*]
                                                        </div>
                                                            <br>
                                                            <p style=""font-family: sans-serif; font-size: 13px !important; font-weight: normal; margin: 0; Margin-bottom: 15px; text-align: center;"">
                                                                [*TEST*]
                                                            </p>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
                    </tr>
                </table>
            </body>
            </html>";

    }
}
