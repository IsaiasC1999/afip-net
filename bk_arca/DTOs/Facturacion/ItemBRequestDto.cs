using bk_arca.Enums;
using System.ComponentModel.DataAnnotations;

namespace bk_arca.DTOs.Facturacion
{
    public class ItemBRequestDto
    {
        // Código MTX de AFIP (si lo usás)
        [MaxLength(50)]
        public string? CodigoMtx { get; set; }

        // Código interno
        [MaxLength(50)]
        public string? Codigo { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        // 7 = UNIDAD por defecto
        [Required]
        public UnidadMtx UnidadMtx { get; set; } = UnidadMtx.Unidad;

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal Cantidad { get; set; }

        // FACTURA B: precio unitario CON IVA incluido
        [Required]
        [Range(0.00, double.MaxValue)]
        public decimal PrecioUnitarioConIva { get; set; }

        // Para el resumen (p.e. 5 = 21%)
        [Required]
        public CondicionIVA CodigoCondicionIVA { get; set; } = CondicionIVA.Gravado21;

        // Monto total del ítem (precio * cantidad - bonificación)
        [Required]
        [Range(0.00, double.MaxValue)]
        public decimal ImporteItem { get; set; }

        // Bonificación opcional
        [Range(0.00, double.MaxValue)]
        public decimal? ImporteBonificacion { get; set; }
    }
}
