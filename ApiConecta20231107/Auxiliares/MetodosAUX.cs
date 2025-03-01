using API.DTOs;
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

        public string MandarCorreoPass(string usuario, string correo, string nuevoPass)
        {
            string result = string.Empty;
            try
            {  
                string body = correoPassReestablecida;
                body = body.Replace("[*USUARIO*]", usuario);
                body = body.Replace("[*PASSWORD*]", nuevoPass);
                body = body.Replace("[*TEST*]", "*******************Correo enviado desde pruebas, por favor hacer caso omiso");
                MandarCorreo("Contraseña actualizada", correo, body);
                result = Constantes.RESPUESTA_OK;
            }
            catch (Exception e)
            {
                var debug = e;
            }
            return result;
        }

        public string MandarCorreo(string asunto, string destinatario, string cuerpo)
        {

            string result = string.Empty;
            try
            {
                //aqui puede ir su logica para mandar correos

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
