# 📄 Facturación — **Factura B** (AFIP MTXCA)

Web API para **autorizar Facturas Tipo B** contra AFIP (servicio **MTXCA**).  
Los ítems trabajan con **precio unitario con IVA incluido**. El armado del `ComprobanteType` se realiza dentro del **servicio de facturación**.

---

## 🚀 Endpoint

| Método | Ruta             | Descripción                       |
|-------:|------------------|-----------------------------------|
|  POST  | `/facturacion/b` | Autoriza una **Factura Tipo B**   |

**Headers**
- `Content-Type: application/json`  
- `Authorization: Bearer <token>` *(solo si tu API tiene auth propia; **no** es el WSAA)*

> Si tu controller usa otra base de ruta, ajustá los ejemplos.

---

## 📤 Body de ejemplo (cópialo tal cual)

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
```

🧪 Cómo probar
cURL
bash
Copiar código
# Reemplazá BASE_URL por tu URL real, p. ej.: http://localhost:5080
curl -X POST "BASE_URL/facturacion/b" \
  -H "Content-Type: application/json" \
  -d @factura-b.json
Tip: guardá el JSON anterior como factura-b.json, o pegalo directo con -d '...json...'.

Postman
Método: POST

URL: BASE_URL/facturacion/b

Headers: Content-Type: application/json

Body: raw (JSON) → pega el JSON de arriba

Enviar

📥 Respuesta (ejemplo)
json
Copiar código
{
  "resultado": "A",
  "cae": "70412345678901",
  "fechaVencimientoCAE": "20251009",
  "numeroComprobante": 5,
  "observaciones": []
}
resultado: A (aprobada) o R (rechazada)

cae, fechaVencimientoCAE: presentes si fue aprobada

observaciones: códigos/leyendas devueltos por AFIP

⚖️ Reglas clave para Factura B
Cada ítem trae precio unitario con IVA incluido.

No enviar importeIVA por ítem.

No enviar importeOtrosTributos = 0 → evita error 114.

Consumidor Final: tipoDocumentoReceptor = 99 y numeroDocumentoReceptor = "0".

condicionIVAReceptor se usa para el resumen (p. ej., 5 = 21%).

Si numeroComprobante es null, el servicio consulta el último autorizado y suma 1.

📁 Estructura del proyecto
Estructura real según la solución AFIP-API (proyecto bk_arca):

graphql
Copiar código
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
Puntos clave

Proxy SOAP (MTXCA): Connected Services/referencias_arca_ws.

Endpoint: Controllers/BillingController.cs.

DTOs: DTOs/Facturacion.

Enums reutilizables: Enums.

Lógica + mapeo FacturaBRequestDto → ComprobanteType: services/FacturacionService.cs.

Interfaz del servicio: services/Interfaces/IFacturacionService.cs.

▶️ Ejecutar en local
bash
Copiar código
dotnet restore
dotnet build
dotnet run --project bk_arca
