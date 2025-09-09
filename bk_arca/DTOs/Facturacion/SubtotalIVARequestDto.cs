using bk_arca.Enums;
using System.ComponentModel.DataAnnotations;

namespace bk_arca.DTOs.Facturacion
{
    public class SubtotalIVARequestDto
    {
        [Required]
        public CondicionIVA Codigo { get; set; } = CondicionIVA.Gravado21;

        [Required]
        [Range(0.00, double.MaxValue)]
        public decimal Importe { get; set; }
    }
}
