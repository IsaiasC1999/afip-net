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

## Respuesta 


```json
{
  "resultado": "A",
  "cae": "70412345678901",
  "fechaVencimientoCAE": "20251009",
  "numeroComprobante": 5,
  "observaciones": []
}

```


## ▶️ Ejecutar en local

``` bash
dotnet restore
dotnet build
dotnet run --project bk_arca
```
