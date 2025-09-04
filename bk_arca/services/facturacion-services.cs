using referencias_arca_ws;

namespace bk_arca.services
{
    public class facturacion_services
    {

        public MTXCAServicePortTypeClient Client { get; set; } = new MTXCAServicePortTypeClient(MTXCAServicePortTypeClient.EndpointConfiguration.MTXCAServiceHttpSoap11Endpoint);

        //var client = new MTXCAServicePortTypeClient(MTXCAServicePortTypeClient.EndpointConfiguration.MTXCAServiceHttpSoap11Endpoint);

        public AuthRequestType Auth { get; set; } = new AuthRequestType
        {
            token = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8c3NvIHZlcnNpb249IjIuMCI+CiAgICA8aWQgc3JjPSJDTj13c2FhaG9tbywgTz1BRklQLCBDPUFSLCBTRVJJQUxOVU1CRVI9Q1VJVCAzMzY5MzQ1MDIzOSIgdW5pcXVlX2lkPSI1NDM5NTU1NzQiIGdlbl90aW1lPSIxNzU2OTIzOTIxIiBleHBfdGltZT0iMTc1Njk2NzE4MSIvPgogICAgPG9wZXJhdGlvbiB0eXBlPSJsb2dpbiIgdmFsdWU9ImdyYW50ZWQiPgogICAgICAgIDxsb2dpbiBlbnRpdHk9IjMzNjkzNDUwMjM5IiBzZXJ2aWNlPSJ3c210eGNhIiB1aWQ9IlNFUklBTE5VTUJFUj1DVUlUIDIwNDE4NzAzMDMzLCBDTj1wcnVlYmFkb3MiIGF1dGhtZXRob2Q9ImNtcyIgcmVnbWV0aG9kPSIyMiI+CiAgICAgICAgICAgIDxyZWxhdGlvbnM+CiAgICAgICAgICAgICAgICA8cmVsYXRpb24ga2V5PSIyMDQxODcwMzAzMyIgcmVsdHlwZT0iNCIvPgogICAgICAgICAgICA8L3JlbGF0aW9ucz4KICAgICAgICA8L2xvZ2luPgogICAgPC9vcGVyYXRpb24+Cjwvc3NvPgo=",
            sign = "eAEAVONoVAYVI+I1Yl2Ug5op5hOKo+zrnJymgVK6lueqDRHZVrWCbvrLLexv/4HtjYKXCzm8AlthFoWigHZGX7Zd9BSpjlB3B4kXI8YO6+EZsn9L9Q/d7/eTtOtZ1emHD3BLJhMj1LqIjbVjJsa1v6j7acWh7uI1Rtcm/4X4pW0=",
            cuitRepresentada = 20418703033
        };
        




        public async Task<autorizarComprobanteResponse> FacturacionTipoB()
        {

            // Espera la llamada asíncrona para obtener la respuesta
            var catResponse = await Client.consultarCondicionesIVAAsync(new consultarCondicionesIVARequest
            {
                authRequest = Auth
            });

            var resu = await Client.consultarCondicionesIVAReceptorAsync(new consultarCondicionesIVAReceptorRequest
            {
                authRequest = Auth,
                consultaCondicionesIVAReceptorRequest = new ConsultaCondicionesIVARequestType
                {
                    // Asigna aquí las propiedades requeridas, por ejemplo:
                    codigoTipoComprobante = 6, // ejemplo: DNI
                                               // ejemplo: CUIT o DNI del receptor
                }
            });

            var ultimoComprobante = await Client.consultarUltimoComprobanteAutorizadoAsync(new consultarUltimoComprobanteAutorizadoRequest
            {
                authRequest = Auth,
                consultaUltimoComprobanteAutorizadoRequest = new ConsultaUltimoComprobanteAutorizadoRequestType
                {
                    codigoTipoComprobante = 6,
                    numeroPuntoVenta = 4000
                }
            });


            var comp = new ComprobanteType
            {
                // Encabezado
                codigoTipoComprobante = 6,          // 6 = Factura B
                numeroPuntoVenta = 4000,
                numeroComprobante = ultimoComprobante.numeroComprobante + 1, // consultá antes el último autorizado
                fechaEmision = DateTime.Today,
                fechaEmisionSpecified = true,
                codigoConcepto = 1,                 // 1 = Productos

                // Receptor - Consumidor Final
                codigoTipoDocumento = 99,           // 99 = CF
                numeroDocumento = 0,
                condicionIVAReceptor = 5,           // usar el valor devuelto por consultarCondicionesIVAReceptor para CF
                condicionIVAReceptorSpecified = true,

                // Totales
                importeGravado = 100.00m,
                importeGravadoSpecified = true,
                importeNoGravado = 0.00m,
                importeNoGravadoSpecified = true,
                importeExento = 0.00m,
                importeExentoSpecified = true,

                // NO informar otros tributos si son 0 (evita error 114)
                // importeOtrosTributos = 0.00m;
                // importeOtrosTributosSpecified = true;  // <- NO enviar

                importeSubtotal = 100.00m,
                importeTotal = 121.00m,

                // Moneda
                codigoMoneda = "PES",
                cotizacionMoneda = 1.00m,
                cotizacionMonedaSpecified = true,

                // Ítems
                arrayItems = new[]
          {
                new ItemType
                {
                    // Código de Producto/Servicio (MTXCA)
                    codigoMtx = "P0001",
                    // (Opcional) tu código interno
                    codigo = "P0001",
                    descripcion = "Producto de prueba",
                    // Unidad de medida (tabla MTX). Ej: 7 = UNIDAD
                    unidadesMtx = 7,
                    unidadesMtxSpecified = true,
                    cantidad = 1.00m,
                    cantidadSpecified = true,
                    // FACTURA B: precio unitario CON IVA incluido
                    precioUnitario = 121.00m,
                    precioUnitarioSpecified = true,
                    // IVA del ítem NO se informa en B (no mandar importeIVA)
                    codigoCondicionIVA = 5,      // 21% (se usa para el resumen)
                    // importeIVA = 21.00m;       // <- NO enviar en B
                    // importeIVASpecified = true;
                    // Importe del ítem = precio (con IVA) * cantidad - bonificación
                    importeItem = 121.00m
                    // Si más adelante aplicás bonificación:
                    // importeBonificacion = X;
                    // importeBonificacionSpecified = true;
                }
            },

                // Resumen de IVA (válido en B para validar totales)
                arraySubtotalesIVA = new[]
          {
                new SubtotalIVAType
                {
                    codigo = 5,        // 21%
                    importe = 21.00m
                }
            }
                // NO enviar codigoAutorizacion / CAE en la solicitud
            };

            var response = await Client.autorizarComprobanteAsync(new autorizarComprobanteRequest
            {
                authRequest = Auth,
                comprobanteCAERequest = comp
            });

            return response;


        }


    }
}
