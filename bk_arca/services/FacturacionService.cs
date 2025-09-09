using bk_arca.DTOs.Facturacion;
using bk_arca.Enums;
using bk_arca.services.Interfaces;
using referencias_arca_ws;

namespace bk_arca.services
{
    public class FacturacionService : IFacturacionService
    {

        public MTXCAServicePortTypeClient Client { get; set; } = new MTXCAServicePortTypeClient(MTXCAServicePortTypeClient.EndpointConfiguration.MTXCAServiceHttpSoap11Endpoint);

        //var client = new MTXCAServicePortTypeClient(MTXCAServicePortTypeClient.EndpointConfiguration.MTXCAServiceHttpSoap11Endpoint);

        public AuthRequestType Auth { get; set; } = new AuthRequestType
        {
            token = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8c3NvIHZlcnNpb249IjIuMCI+CiAgICA8aWQgc3JjPSJDTj13c2FhaG9tbywgTz1BRklQLCBDPUFSLCBTRVJJQUxOVU1CRVI9Q1VJVCAzMzY5MzQ1MDIzOSIgdW5pcXVlX2lkPSI4MzM0NTYzOTciIGdlbl90aW1lPSIxNzU3NDIxMjM3IiBleHBfdGltZT0iMTc1NzQ2NDQ5NyIvPgogICAgPG9wZXJhdGlvbiB0eXBlPSJsb2dpbiIgdmFsdWU9ImdyYW50ZWQiPgogICAgICAgIDxsb2dpbiBlbnRpdHk9IjMzNjkzNDUwMjM5IiBzZXJ2aWNlPSJ3c210eGNhIiB1aWQ9IlNFUklBTE5VTUJFUj1DVUlUIDIwNDE4NzAzMDMzLCBDTj1wcnVlYmFkb3MiIGF1dGhtZXRob2Q9ImNtcyIgcmVnbWV0aG9kPSIyMiI+CiAgICAgICAgICAgIDxyZWxhdGlvbnM+CiAgICAgICAgICAgICAgICA8cmVsYXRpb24ga2V5PSIyMDQxODcwMzAzMyIgcmVsdHlwZT0iNCIvPgogICAgICAgICAgICA8L3JlbGF0aW9ucz4KICAgICAgICA8L2xvZ2luPgogICAgPC9vcGVyYXRpb24+Cjwvc3NvPgo=",
            sign = "fZNZFQuIECmlKODEu0TDqCLDm2QoIuAYd0ol19pxt9bjGVWFZ+PAD6ZNT6EYUyecfCPoWvPXMYF5JU960INSkq3smsAXLICrEk8ujcZ7Ytc5bilLjtWIkfPv1IGaLPBWB+QAiU4VRCLskufgMR/mM1WRNVAdRJpbmF9rw24xbrE=",
            cuitRepresentada = 20418703033
        };




        public async Task<autorizarComprobanteResponse> AutorizarFacturaBAsync(FacturaBRequestDto dto)
        {
            // Validaciones mínimas de negocio
            if (dto.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("La factura debe contener al menos un ítem.");

            // Fecha por defecto
            var fecha = dto.FechaEmision?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;

            // Número de comprobante
            int numeroComprobante;
            if (dto.NumeroComprobante.HasValue)
            {
                numeroComprobante = dto.NumeroComprobante.Value;
            }
            else
            {
                var ultimo = await Client.consultarUltimoComprobanteAutorizadoAsync(
                    new consultarUltimoComprobanteAutorizadoRequest
                    {
                        authRequest = Auth,
                        consultaUltimoComprobanteAutorizadoRequest = new ConsultaUltimoComprobanteAutorizadoRequestType
                        {
                            codigoTipoComprobante = (int)TipoComprobante.FacturaB,
                            numeroPuntoVenta = dto.NumeroPuntoVenta
                        }
                    });

                // Fix: Convert 'long' to 'int' explicitly
                numeroComprobante = Convert.ToInt32(ultimo.numeroComprobante);
            }

            RecalcularTotalesFacturaB(dto);

            // Mapear a ComprobanteType (sin enviar campos en 0 innecesarios)
            var comp = MapToComprobanteType(dto, numeroComprobante, fecha);

            // Autorizar
            var resp = await Client.autorizarComprobanteAsync(new autorizarComprobanteRequest
            {
                authRequest = Auth,
                comprobanteCAERequest = comp
            });

            return resp;
        }




        // ----------------- Helpers privados -----------------

        /// <summary>
        /// Factura B: cada item trae PrecioUnitario CON IVA. 
        /// Recomputamos:
        /// - ImporteItem = (precio - bonificación) * cantidad
        /// - ImporteGravado = suma de base imponible neta de IVA por alícuota
        /// - SubtotalesIVA = opcional para validación; si dto ya lo trae, respetamos
        /// Nota: si tenés múltiples alícuotas, agregá la lógica por grupo.
        /// </summary>
        private static void RecalcularTotalesFacturaB(FacturaBRequestDto dto)
        {
            // 1) Recalcular importe por ítem (si no viene)
            foreach (var i in dto.Items)
            {
                var bonif = i.ImporteBonificacion ?? 0m;
                if (i.ImporteItem <= 0)
                {
                    i.ImporteItem = Math.Round((i.PrecioUnitarioConIva - bonif) * i.Cantidad, 2);
                }
            }

            // 2) Total bruto (con IVA)
            var totalConIva = dto.Items.Sum(x => x.ImporteItem);

            // 3) Si no te pasaron subtotales IVA, podemos construirlos simples (p.e. todo 21%).
            //    Si ya vienen en dto.SubtotalesIVA, los respetamos.
            if (dto.SubtotalesIVA == null || dto.SubtotalesIVA.Count == 0)
            {
                // Caso simple: un único código IVA en todos los ítems -> tomamos el primero
                var cod = dto.Items.GroupBy(x => x.CodigoCondicionIVA)
                                   .Select(g => g.Key)
                                   .SingleOrDefault();

                // Si hay mezcla de alícuotas, acá deberías agrupar y calcular por grupo (extender según tu negocio)
                if (cod == 0) cod = CondicionIVA.Gravado21;

                // Obtener alícuota numérica
                var alicuta = GetAlicuota(cod); // 0.21m, 0.105m, etc.

                // Base imponible aproximada: total / (1 + alícuota)
                var baseImponible = Math.Round(totalConIva / (1 + alicuta), 2);
                var iva = Math.Round(totalConIva - baseImponible, 2);

                dto.ImporteGravado = baseImponible;
                dto.ImporteNoGravado = 0m;
                dto.ImporteExento = 0m;

                dto.ImporteSubtotal = baseImponible; // si no hay otros tributos ni exentos/no gravados al subtotal
                dto.ImporteTotal = totalConIva;

                dto.SubtotalesIVA = new()
            {
                new SubtotalIVARequestDto { Codigo = cod, Importe = iva }
            };
            }
            else
            {
                // Si viene el detalle de IVA, ajustamos gravado y totales en base a esos subtotales
                var totalIva = dto.SubtotalesIVA.Sum(x => x.Importe);
                var baseImponible = Math.Round(totalConIva - totalIva, 2);

                dto.ImporteGravado = baseImponible;
                // Respetamos lo que venga en NoGravado/Exento si lo enviaste; si no, mantenemos 0
                dto.ImporteSubtotal = baseImponible;
                dto.ImporteTotal = totalConIva;
            }
        }

        private static decimal GetAlicuota(CondicionIVA cod)
        {
            return cod switch
            {
                CondicionIVA.Gravado27 => 0.27m,
                CondicionIVA.Gravado21 => 0.21m,
                CondicionIVA.Gravado10_5 => 0.105m,
                CondicionIVA.Gravado0 => 0.00m,
                CondicionIVA.Exento => 0.00m,
                CondicionIVA.NoGravado => 0.00m,
                _ => 0.21m
            };
        }

        /// <summary>
        /// Mapeo centralizado dentro del servicio (lo pediste así).
        /// Cuida de NO enviar campos en 0 que generen error (ej. OtrosTributos=0).
        /// </summary>
        private static ComprobanteType MapToComprobanteType(FacturaBRequestDto dto, int numeroComprobante, DateTime fechaEmision)
        {
            var comp = new ComprobanteType
            {
                // Encabezado
                codigoTipoComprobante = (int)TipoComprobante.FacturaB,
                numeroPuntoVenta = dto.NumeroPuntoVenta,
                numeroComprobante = numeroComprobante + 1,
                fechaEmision = fechaEmision,
                fechaEmisionSpecified = true,
                codigoConcepto = (short)dto.ComprobanteConcepto,

                // Receptor
                codigoTipoDocumento = (short)dto.TipoDocumentoReceptor,
                numeroDocumento = long.TryParse(dto.NumeroDocumentoReceptor, out var nd) ? nd : 0L,
                condicionIVAReceptor = (short)dto.CondicionIVAReceptor,
                condicionIVAReceptorSpecified = true,

                // Totales (en B el precio es CON IVA; evitamos mandar 0s innecesarios)
                importeGravado = dto.ImporteGravado,
                importeGravadoSpecified = dto.ImporteGravado > 0,
                importeNoGravado = dto.ImporteNoGravado,
                importeNoGravadoSpecified = dto.ImporteNoGravado > 0,
                importeExento = dto.ImporteExento,
                importeExentoSpecified = dto.ImporteExento > 0,

                importeSubtotal = dto.ImporteSubtotal,
                importeTotal = dto.ImporteTotal,

                // Moneda
                codigoMoneda = dto.CodigoMoneda,
                cotizacionMoneda = dto.CotizacionMoneda,
                cotizacionMonedaSpecified = true,

                // Ítems
                arrayItems = dto.Items.Select(i => new ItemType
                {
                    codigoMtx = i.CodigoMtx,
                    codigo = i.Codigo,
                    descripcion = i.Descripcion,
                    unidadesMtx = (int)i.UnidadMtx,
                    unidadesMtxSpecified = true,
                    cantidad = i.Cantidad,
                    cantidadSpecified = true,

                    // B: precio CON IVA
                    precioUnitario = i.PrecioUnitarioConIva,
                    precioUnitarioSpecified = true,

                    // No enviar importeIVA en B; solo código para el resumen
                    codigoCondicionIVA = (short)i.CodigoCondicionIVA,

                    importeItem = i.ImporteItem,
                    importeBonificacion = i.ImporteBonificacion ?? 0m,
                    importeBonificacionSpecified = i.ImporteBonificacion.HasValue
                }).ToArray(),

                // Resumen de IVA (opcional, útil para validar totales)
                arraySubtotalesIVA = dto.SubtotalesIVA?.Select(s => new SubtotalIVAType
                {
                    codigo = (short)s.Codigo,
                    importe = s.Importe
                }).ToArray()
            };

            // MUY IMPORTANTE: NO enviar importeOtrosTributos=0
            // comp.importeOtrosTributos = 0m;
            // comp.importeOtrosTributosSpecified = true; // <- NO HACER

            return comp;
        }


    }
}
