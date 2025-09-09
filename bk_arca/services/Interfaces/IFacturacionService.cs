using bk_arca.DTOs.Facturacion;
using referencias_arca_ws;

namespace bk_arca.services.Interfaces
{
    public interface IFacturacionService
    {
        Task<autorizarComprobanteResponse> AutorizarFacturaBAsync(FacturaBRequestDto dto);
    }
}
