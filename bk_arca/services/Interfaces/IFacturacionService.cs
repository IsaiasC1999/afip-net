using bk_arca.DTOs.Facturacion.FacturaA;
using bk_arca.DTOs.Facturacion.FacturaB;
using referencias_arca_ws;

namespace bk_arca.services.Interfaces
{
    public interface IFacturacionService
    {
        Task<autorizarComprobanteResponse> AutorizarFacturaBAsync(FacturaBRequestDto dto);

        Task<autorizarComprobanteResponse> AutorizarFacturaAAsync(FacturaARequestDto dto);
    }
}
