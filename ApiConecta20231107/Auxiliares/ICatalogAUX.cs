using API.DTOs;

namespace API.Auxiliares
{
    public interface ICatalogAUX
    {
        List<ComboDTO> Combos(string tipo);
    }
}
