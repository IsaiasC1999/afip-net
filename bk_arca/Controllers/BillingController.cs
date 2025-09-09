using bk_arca.DTOs.Facturacion;
using bk_arca.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using referencias_arca_ws;

namespace bk_arca.Controllers
{
    [Route("api/facturacion")]
    [ApiController]
    public class BillingController : ControllerBase
    {

        

        public FacturacionService service { get; set; }

        public BillingController()
        {
            service = new FacturacionService();
        }

        

        [HttpPost("b")]
        public async Task<ActionResult<autorizarComprobanteResponse>> PostFacturaB([FromBody] FacturaBRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var resp = await service.AutorizarFacturaBAsync(dto);
            return Ok(resp.comprobanteResponse);
        }

    }
}
