# 📄 Facturación — **Factura B** (AFIP MTXCA)

Web API para **autorizar Facturas Tipo B** contra AFIP (servicio **MTXCA**).  
Los ítems trabajan con **precio unitario con IVA incluido**. El armado del `ComprobanteType` se realiza dentro del **servicio de facturación**.

---

## 🚀 Endpoint

| Método | Ruta             | Descripción                       |
|-------:|------------------|-----------------------------------|
|  POST  | `/facturacion/b` | Autoriza una **Factura Tipo B**   
|  POST  | `/facturacion/a` | Autoriza una **Factura Tipo A**   |

**Headers**
- `Content-Type: application/json`  
- `Authorization: Bearer <token>` *(solo si tu API tiene auth propia; **no** es el WSAA)*

> Si tu controller usa otra base de ruta, ajustá los ejemplos.

---

## 📤 Body de ejemplo factura b (cópialo tal cual)

```json
{
  "numeroPuntoVenta": 4000,
  "tipoComprobante": 6,
  "numeroComprobante": null,
  "fechaEmision": "2025-10-02"  <--- actualizar a fecha actual,
  "comprobanteConcepto": 1,
  "tipoDocumentoReceptor": 99,
  "numeroDocumentoReceptor": "0",
  "condicionIVAReceptor": 5,
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
    },
        {
      "codigoMtx": "P0001",
      "codigo": "P0002",
      "descripcion": "Producto de prueba",
      "unidadMtx": 7,
      "cantidad": 2.0,
      "precioUnitarioConIva": 200.00,
      "codigoCondicionIVA": 5,
      "importeItem": 400.00
    }
]
  
}```

## 📥 Respuesta (ejemplo)

```json
{
  "resultado": "A",
  "cae": "70412345678901",
  "fechaVencimientoCAE": "20251009",
  "numeroComprobante": 5,
  "observaciones": []
}
```

- `resultado`: `A` (aprobada) o `R` (rechazada)  
- `cae`, `fechaVencimientoCAE`: presentes si fue aprobada  
- `observaciones`: códigos/leyendas devueltos por AFIP

---

## 🗂️ Estructura del proyecto

```text
bk_arca/
├─ Connected Services/
│  └─ referencias_arca_ws/
│     ├─ ConnectedService.json
│     └─ Reference.cs            # Proxy SOAP generado (MTXCA)
├─ Controllers/
│  └─ BillingController.cs       # Expone POST /facturacion/b (ajustar si difiere)
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
│  ├─ FacturacionService.cs      # Mapeo DTO → ComprobanteType + llamada a AFIP
│  └─ facturacion-services.cs    # (si lo usás como archivo adicional)
├─ Properties/
│  └─ launchSettings.json        # Puertos/Base URL de desarrollo
├─ appsettings.json
├─ Program.cs
├─ WeatherForecast.cs
├─ bk_arca.http                  # (opcional) Requests para VS Code
└─ (otros)
```

---


## 🚀 Requisitos

- .NET SDK 7 u 8 instalado.
- Proyecto **bk_arca** compilable.
- Configuración de **WSAA/MTXCA** para Homologación/Producción (tokens/certificados).
- Base URL de la API (por ej. `http://localhost:5080`).

---

## ▶️ Ejecutar en local

``` bash
dotnet restore
dotnet build
dotnet run --project bk_arca
```
