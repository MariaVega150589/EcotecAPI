using ProjectAPI.Application.Common.Models;

namespace ProjectAPI.Application.BankResume.Models;

public class CatalogosPredefinidosDto
{
    public List<ItemDto> EstatusRpa { get; set; }
    public List<ItemDto> Permiso { get; set; }
    public List<ItemDto> TipoToken { get; set; }
}