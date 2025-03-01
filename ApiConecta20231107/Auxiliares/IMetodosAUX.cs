using Data;

namespace API.Auxiliares
{
    public interface IMetodosAUX
    {
        List<string[]> HistorialGenerico(string tabla, string filtroTabla, string filtroEmpleado);
        ResponseWebApiMulti PostWebAPiUsersMultiLogin<T>(T entity, string url);
        string MandarCorreoNuevoUsuario(string nombre, string usuario, string correo, string sistemas);
        string MandarCorreoPass( string usuario, string correo, string nuevoPass);
    }
}
