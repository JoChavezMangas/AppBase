namespace API.Auxiliares
{
    public class Constantes
    {
        public const string HISTORIALEMPLEADO_NUEVOCAMBIO_CREAR = "Regsitro de empleado";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_CREAR = "#Creacion";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_BONOTRIMESTRAL = "#BonoTrimestral";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_PUESTO = "#Puesto";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_AREA = "#Area";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_EMPRESA = "#Empresa";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_ROL = "#Rol";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_VACACIONES = "#Vacaciones";
        public const string HISTORIALEMPLEADO_IDENTIFICADOR_DIASESPECIALES = "#DiasEspeciales";

        public const string ESTATUS_PERIODO_VACACIONES_DISPONIBLE = "#Disponible";
        public const string ESTATUS_PERIODO_VACACIONES_CERRADO = "#Cerrado";
        public const string ESTATUS_PERIODO_VACACIONES_REABIERTO = "#Reabierto";

        public const string ESTATUS_SOLICITUD_VACACIONES_AUT = "#Autorizado";
        public const string ESTATUS_SOLICITUD_VACACIONES_REC = "#Rechazado";
        public const string ESTATUS_SOLICITUD_VACACIONES_PEN = "#Pendiente";
        public const string ESTATUS_SOLICITUD_VACACIONES_PENRH = "#PendienteRH";

        public const string ROLES_ADMIN = "Admin";
        public const string ROLES_OPERADOR = "Operador";
        public const string ROLES_GERENTE = "Gerente";
        public const string ROLES_RH = "RecursosHumanos";
        public const string ROLES_USUARIO = "Usuario";
        public const string ROLES_CLAIMS_ADMIN = "esAdmin";
        public const string ROLES_CLAIMS_OPERADOR = "esOp";
        public const string ROLES_CLAIMS_USUARIO = "esUs";


        public const string OTRO_SISTEMA_MULTILOGIN = "#MultiLogin";
        public const string OTRO_SISTEMA_AGENTESOC = "#AgenteSOC";
        public const string OTRO_SISTEMA_BROKERMASTER = "#BrokersMaster";
        public const string OTRO_SISTEMA_CAPTURA = "#CVeloz";
        public const string OTRO_SISTEMA_CREDIHIPO = "#CrediHipotecario";
        public const string OTRO_SISTEMA_RESUELVEME = "#Resuelveme";
        public const string OTRO_SISTEMA_SICAFI = "#Sicafi";
        public const string OTRO_SISTEMA_SISEC = "#Sisec";
        public const string OTRO_SISTEMA_VALIDOC = "#Validoc";
        public const string OTRO_SISTEMA_CONECTA = "#Conecta";

        public const string MODELO_OTRO_SISTEMA_MULTILOGIN_ACTIVAR = "activarMultilogin";

        //public const string OTRO_SISTEMA_COMPARADOR     = "#Comparador";


        public const string RESPUESTA_OK = "OK";
        public const string RESPUESTA_REGISTRO_NO_ENCONTRADO = "RegistroNOEncontrado";
        public const string RESPUESTA_NO_AUT = "NoAutorizado";
        public const string RESPUESTA_DOC_FALTANTE = "Por favor, ingresa un documento";
        public const string RESPUESTA_RFC_DUPLICADO = "Ya hay un empleado con este RFC";
        public const string RESPUESTA_CURP_DUPLICADO = "Ya hay un empleado con esta CURP";
        public const string RESPUESTA_CORREO_DUPLICADO = "Ya hay un empleado con este correo";
        public const string RESPUESTA_PETICION_NULL = "Selecciona un tipo de petición";
        public const string RESPUESTA_DIASTOTALES_CERO = "Los días de la solicitud no pueden ser 0";

        public const string RESPUESTA_ERROR_EMPLEADO_MULTILOGIN = "No sue pudo mandar el empleado a otros sitemas";
        public const string RESPUESTA_ERROR_EMPLEADO_DATOS = "Por favor, verifica los datos base del empleado";
        public const string RESPUESTA_ERROR_EMPLEADO_DIRECCION = "Por favor, verifica la direccion del empleado";
        public const string RESPUESTA_ERROR_EMPLEADO_CONTRATACION = "Por favor, verifica el tipo de contratacion del empleado";
        public const string RESPUESTA_ERROR_EMPLEADO_BANCARIO = "Por favor, verifica los datos bancarios empleado";
        public const string RESPUESTA_ERROR_EMPLEADO_NOMINA = "Por favor, verifica los datos de nomina del empleado";
        public const string RESPUESTA_ERROR_EMPLEADO_CONTACTO = "Por favor, verifica los datos de contacto del empleado";
        public const string RESPUESTA_ERROR = "ERROR";


    }
}
