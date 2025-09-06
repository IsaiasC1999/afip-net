using bk_arca.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bk_arca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {

        public facturacion_services services { get; set; }

        public BillingController()
        {
            services = new facturacion_services();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await services.FacturacionTipoB();
            return Ok(result);
        }


    }
}
