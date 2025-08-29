// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");



using ServiceReference1;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Ensure the service reference is properly added to the project
        var client = new MTXCAServicePortTypeClient(MTXCAServicePortTypeClient.EndpointConfiguration.MTXCAServiceHttpSoap11Endpoint);

        var auth = new AuthRequestType
        {
            token = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8c3NvIHZlcnNpb249IjIuMCI+CiAgICA8aWQgc3JjPSJDTj13c2FhaG9tbywgTz1BRklQLCBDPUFSLCBTRVJJQUxOVU1CRVI9Q1VJVCAzMzY5MzQ1MDIzOSIgdW5pcXVlX2lkPSIyMzI0MjAxODQzIiBnZW5fdGltZT0iMTc1NjQ1NTk2NiIgZXhwX3RpbWU9IjE3NTY0OTkyMjYiLz4KICAgIDxvcGVyYXRpb24gdHlwZT0ibG9naW4iIHZhbHVlPSJncmFudGVkIj4KICAgICAgICA8bG9naW4gZW50aXR5PSIzMzY5MzQ1MDIzOSIgc2VydmljZT0id3NtdHhjYSIgdWlkPSJTRVJJQUxOVU1CRVI9Q1VJVCAyMDQxODcwMzAzMywgQ049cHJ1ZWJhZG9zIiBhdXRobWV0aG9kPSJjbXMiIHJlZ21ldGhvZD0iMjIiPgogICAgICAgICAgICA8cmVsYXRpb25zPgogICAgICAgICAgICAgICAgPHJlbGF0aW9uIGtleT0iMjA0MTg3MDMwMzMiIHJlbHR5cGU9IjQiLz4KICAgICAgICAgICAgPC9yZWxhdGlvbnM+CiAgICAgICAgPC9sb2dpbj4KICAgIDwvb3BlcmF0aW9uPgo8L3Nzbz4K",
            sign = "HuDKPA09wtML3ORrmLVvHULcdXAtqtrBgZkz0Hxt30giTbEo+R1cUKBGZlqoMrzeyO6+waxkJ/jSo9PYGsukM5MPOwc0d22YtRYitYnZoTyJsGETCV4wRqSujkbkRa+kdSepakMvsv0396k3LUEkPSckPOfJY8P1Fx3iE3ScMGo=",
            cuitRepresentada = 20418703033
        };

        // Move the properties to the correct object (ComprobanteType)
        var comp = new ComprobanteType
        {
            codigoTipoComprobante = 1,
            numeroPuntoVenta = 4000,
            numeroComprobante = 123,
            fechaEmision = DateTime.Now,
            codigoTipoDocumento = 80,
            numeroDocumento = 30000000007,
            importeTotal = 100.00M,
            codigoMoneda = "PES",
            cotizacionMoneda = 1
        };

        var req = new autorizarComprobanteRequest
        {
            authRequest = auth,
            comprobanteCAERequest = comp
        };

        var resp = await client.autorizarComprobanteAsync(req);

        Console.WriteLine($"CAE: {resp.comprobanteResponse}");
        Console.WriteLine($"Resultado: {resp.comprobanteResponse}");
    }
}
