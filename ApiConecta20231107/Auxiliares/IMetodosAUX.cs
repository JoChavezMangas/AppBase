using API.DTOs.OtrosSistemas;
using Data;

namespace API.Auxiliares
{
    public interface IMetodosAUX
    {
        List<string[]> HistorialGenerico(string tabla, string filtroTabla, string filtroEmpleado);
        string UploadFiles(List<IFormFile> files,int solicitudId ,string _contexEmpleadoId);
        string MandarOtroSistema(GeneralDTO generalDTO,string url, string type = "post");
        ResponseWebApiMulti PostWebAPiUsersMultiLogin<T>(T entity, string url);
        string MandarCorreoNuevoUsuario(string nombre, string usuario, string correo, string sistemas);
        string MandarCorreoPass( string usuario, string correo, string nuevoPass);
        Task<GeneralDTO> ObtenerOtroSistema(string id, string url, string sistema);
    }
}
