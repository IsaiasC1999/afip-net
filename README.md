# Facturación — Factura B (AFIP MTXCA)

Este endpoint autoriza **Facturas Tipo B** usando el servicio **MTXCA** de AFIP.  
Trabaja con **precios de ítems con IVA incluido** y arma el `ComprobanteType` dentro de un **servicio de facturación**.

---

## 📌 Endpoint

- **POST** `/facturacion/b`  
  > (Si tu controller usa otra ruta, ajustá este valor. En los ejemplos se usa `/facturacion/b`.)

### Headers
- `Content-Type: application/json`
- `Authorization: Bearer <token>` *(si tu API tiene auth propia; **no** es el WSAA)*

---

## 📤 Body de ejemplo (JSON para pegar)

> Pega **exactamente** este JSON en el cuerpo del request:

```json
{
  "numeroPuntoVenta": 4000,
  "tipoComprobante": 6,
  "numeroComprobante": null,
  "fechaEmision": "2025-09-09",
  "comprobanteConcepto": 1,
  "tipoDocumentoReceptor": 99,
  "numeroDocumentoReceptor": "0",
  "condicionIVAReceptor": 5,
  "importeGravado": 100.00,
  "importeNoGravado": 0.00,
  "importeExento": 0.00,
  "importeSubtotal": 100.00,
  "importeTotal": 121.00,
  "codigoMoneda": "PES",
  "cotizacionMoneda": 1.00,
  "items": [
    {
      "codigoMtx": "P0001",
      "codigo": "P0001",
      "descripcion": "Producto de prueba",
      "unidadMtx": 7,
      "cantidad": 1.0,
      "precioUnitarioConIva": 121.00,
      "codigoCondicionIVA": 5,
      "importeItem": 121.00
    }
  ],
  "subtotalesIVA": [
    { "codigo": 5, "importe": 21.00 }
  ]
}


##cURL
# Reemplazá BASE_URL por tu URL real, ej: http://localhost:5080
curl -X POST "BASE_URL/facturacion/b" \
  -H "Content-Type: application/json" \
  -d @factura-b.json


## repuesta (ejemplo)

{
  "resultado": "A",
  "cae": "70412345678901",
  "fechaVencimientoCAE": "20251009",
  "numeroComprobante": 5,
  "observaciones": []
}


# Estructura 
Estructura según la solución AFIP-API con el proyecto bk_arca.

bk_arca/
├─ Connected Services/
│  └─ referencias_arca_ws/
│     ├─ ConnectedService.json
│     └─ Reference.cs                     # Proxy SOAP generado (MTXCA)
├─ Controllers/
│  └─ BillingController.cs                # Expone POST /facturacion/b (ajustar si difiere)
├─ DTOs/
│  └─ Facturacion/
│     ├─ FacturaBRequestDto.cs
│     ├─ ItemBRequestDto.cs
│     └─ SubtotalIVARequestDto.cs
├─ Enums/
│  ├─ Concepto.cs
│  ├─ CondicionIVA.cs
│  ├─ TipoComprobante.cs
│  ├─ TipoDocumento.cs
│  └─ UnidadMtx.cs
├─ services/
│  ├─ Interfaces/
│  │  └─ IFacturacionService.cs
│  ├─ FacturacionService.cs               # Mapeo DTO → ComprobanteType + llamada a AFIP
│  └─ facturacion-services.cs             # (archivo adicional si lo usás)
├─ Properties/
│  └─ launchSettings.json                 # Puertos/base URL de desarrollo
├─ appsettings.json
├─ Program.cs
├─ WeatherForecast.cs
├─ bk_arca.http
└─ (otros)


##Puntos clave según esta estructura

 - El proxy SOAP (MTXCA) se encuentra en Connected Services/referencias_arca_ws.

 - El endpoint vive en Controllers/BillingController.cs.

 - Los DTOs de la factura están en DTOs/Facturacion.

 - Los enums reutilizables en Enums.

 - La lógica de negocio y el mapa FacturaBRequestDto → ComprobanteType están en services/FacturacionService.cs.

 - La interfaz del servicio está en services/Interfaces/IFacturacionService.cs.

# Ejecutar en local 

dotnet restore
dotnet build
dotnet run --project bk_arca

