using DBModel.DB;
using Models.RequestResponse;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UtilPDF
{
    public static class PedidoQuestPdfHelper
    {
        public static byte[] GenerarPdfPorProveedor(List<PedidoDetalleResponse> pedidos, DateTime fechaReporte)
        {
            var agrupado = pedidos
                .GroupBy(p => p.Proveedor)
                .Select(grupo => new PedidoGrupo
                {
                    Proveedor = grupo.Key,
                    Fecha = fechaReporte,
                    Pedidos = grupo.ToList()
                }).ToList();

            var documento = new PedidoDocument(agrupado);
            return documento.GeneratePdf();
        }

        private class PedidoGrupo
        {
            public string Proveedor { get; set; }
            public DateTime Fecha { get; set; }
            public List<PedidoDetalleResponse> Pedidos { get; set; }
        }

        private class PedidoDocument : IDocument
        {
            private readonly List<PedidoGrupo> _grupos;

            public PedidoDocument(List<PedidoGrupo> grupos) => _grupos = grupos;

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontFamily("Helvetica").FontSize(10));
                    page.Header().Text("Reporte de Pedidos por Proveedor")
                        .SemiBold().FontSize(16).FontColor(Colors.Black);

                    page.Content().Column(column =>
                    {
                        foreach (var grupo in _grupos)
                        {
                            column.Item().PaddingBottom(20).Element(c => CrearSeccionProveedor(c, grupo));
                        }
                    });
                });
            }

            private void CrearSeccionProveedor(IContainer container, PedidoGrupo grupo)
            {
                container.Column(col =>
                {
                    col.Item().Text($"Proveedor: {grupo.Proveedor}")
                        .Bold().FontSize(12).FontColor(Colors.Blue.Medium);
                    col.Item().Text($"Fecha del Reporte: {grupo.Fecha:dd/MM/yyyy}")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    foreach (var pedido in grupo.Pedidos)
                    {
                        col.Item().PaddingVertical(10).Element(e => CrearPedidoDetalle(e, pedido));
                    }
                });
            }

            private void CrearPedidoDetalle(IContainer container, PedidoDetalleResponse pedido)
            {
                container.Column(col =>
                {
                    col.Item().Text($"Pedido #{pedido.Id} - Estado: {pedido.Estado}")
                        .SemiBold().FontColor(Colors.Blue.Darken2);

                    if (!string.IsNullOrWhiteSpace(pedido.NombreCliente))
                        col.Item().Text($"Cliente: {pedido.NombreCliente}");

                    if (!string.IsNullOrWhiteSpace(pedido.DescripcionPedido))
                        col.Item().Text($"Descripción: {pedido.DescripcionPedido}")
                            .Italic().FontColor(Colors.Grey.Darken1);

                    col.Item().PaddingTop(5).Element(e => CrearTablaLibros(e, pedido.Detalles));

                    if (!string.IsNullOrWhiteSpace(pedido.DescripcionRecepcion))
                        col.Item().Text($"Notas de Recepción: {pedido.DescripcionRecepcion}")
                            .Italic().FontColor(Colors.Teal.Darken2);
                });
            }

            private void CrearTablaLibros(IContainer container, List<LibroPedidoDetalleDto> detalles)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(); // Título
                        columns.RelativeColumn(0.8f); // ISBN
                        columns.ConstantColumn(60); // Pedida
                        columns.ConstantColumn(60); // Recibida
                        columns.ConstantColumn(70); // Precio
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Título").SemiBold().FontSize(10);
                        header.Cell().Text("ISBN").SemiBold().FontSize(10);
                        header.Cell().Text("Pedida").SemiBold().FontSize(10);
                        header.Cell().Text("Recibida").SemiBold().FontSize(10);
                        header.Cell().Text("P. Unit").SemiBold().FontSize(10);
                    });

                    foreach (var item in detalles)
                    {
                        table.Cell().Text(item.Titulo).FontSize(9);
                        table.Cell().Text(item.Isbn).FontSize(9);
                        table.Cell().Text(item.CantidadPedida.ToString()).FontSize(9);
                        table.Cell().Text((item.CantidadRecibida ?? 0).ToString()).FontSize(9);
                        table.Cell().Text($"S/. {item.PrecioUnitario:F2}").FontSize(9);
                    }
                });
            }

        }
    }
    //Resumen De ingresos PDF
    public static class ResumenIngresoPdfHelper
    {
        public static byte[] GenerarResumenIngresos(
            List<VentaResponse> ventas,
            Dictionary<int, List<DetalleVentaResponse>> detallesPorVenta,
            string vendedor,
            DateTime fechaReporte)
        {
            var documento = new DocumentoResumenIngresos(ventas, detallesPorVenta, vendedor, fechaReporte);
            return documento.GeneratePdf();
        }

        private class DocumentoResumenIngresos : IDocument
        {
            private readonly List<VentaResponse> _ventas;
            private readonly Dictionary<int, List<DetalleVentaResponse>> _detalles;
            private readonly string _vendedor;
            private readonly DateTime _fecha;

            public DocumentoResumenIngresos(List<VentaResponse> ventas,
                                            Dictionary<int, List<DetalleVentaResponse>> detalles,
                                            string vendedor,
                                            DateTime fecha)
            {
                _ventas = ventas;
                _detalles = detalles;
                _vendedor = vendedor;
                _fecha = fecha;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontFamily("Helvetica").FontSize(9));

                    page.Header().Element(ConstruirEncabezado);
                    page.Content().Element(ConstruirTablaVentas);
                });
            }

            private void ConstruirEncabezado(IContainer container)
            {
                container.Column(column =>
                {
                    // Título principal
                    column.Item().Text("Resúmen de ingresos por métodos de pago")
                        .Bold().FontSize(14).FontColor(Colors.Black);

                    column.Item().PaddingTop(10);

                    // Información de la empresa en dos columnas
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text("Empresa: GRUPO EMPRESARIAL KELPAD SRL").FontSize(10);
                            leftColumn.Item().Text("Ruc: 20613771591").FontSize(10);
                            leftColumn.Item().Text($"Vendedor: {_vendedor}").FontSize(10);
                            leftColumn.Item().Text("Estado de caja: Aperturada").FontSize(10);
                        });

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().Text($"Fecha reporte: {_fecha:yyyy-MM-dd}").FontSize(10);
                            rightColumn.Item().Text("Establecimiento: Jr. Calixto 563 - JUNÍN - Huancayo").FontSize(10);
                            rightColumn.Item().Text($"Fecha y hora apertura: {_fecha:yyyy-MM-dd HH:mm:ss}").FontSize(10);
                        });
                    });

                    column.Item().PaddingTop(10);

                    // Totales
                    var totalComprobantes = _ventas.Where(v => v.TipoComprobante != "NOTA DE VENTA").Sum(v => v.TotalPrecio ?? 0);
                    var totalNotas = _ventas.Where(v => v.TipoComprobante == "NOTA DE VENTA").Sum(v => v.TotalPrecio ?? 0);
                    if (totalComprobantes > 0)
                    {
                        column.Item().Text($"Total comprobantes: S/ {totalComprobantes:F2}")
                            .FontSize(10).Bold();
                    }
                    if (totalNotas > 0) {
                        column.Item().Text($"Total notas de venta: S/ {totalNotas:F2}")
                            .FontSize(10).Bold();
                    }


                    column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });
            }

            private void ConstruirTablaVentas(IContainer container)
            {
                container.Table(table =>
                {
                    // Definir columnas con anchos ajustados (total = 535 puntos disponibles)
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(20);  // # 
                        columns.ConstantColumn(80);  // Fecha y hora
                        columns.ConstantColumn(120); // Tipo documento
                        columns.ConstantColumn(60);  // Documento
                        columns.ConstantColumn(70);  // Método de pago
                        columns.ConstantColumn(50);  // Moneda
                        columns.ConstantColumn(45);  // Importe
                        columns.ConstantColumn(40);  // Vuelto
                        columns.ConstantColumn(50);  // Monto
                    });

                    // Encabezados de la tabla
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("#").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha y hora\nemisión").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Tipo documento").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Documento").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Método de\npago").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Moneda").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Importe").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Vuelto").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Monto").SemiBold().FontSize(8);
                    });

                    // Datos de las ventas
                    int contador = 1;
                    foreach (var venta in _ventas)
                    {
                        // Alternar colores de fila
                        var backgroundColor = contador % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                        table.Cell().Background(backgroundColor).Padding(5).Text(contador.ToString()).FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            venta.FechaVenta?.ToString("yyyy-MM-dd\nHH:mm:ss") ?? "").FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            ObtenerTipoDocumentoFormateado(venta.TipoComprobante ?? "")).FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            venta.NroComprobante ?? "").FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            venta.TipoPago ?? "Efectivo").FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text("PEN").FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            $"{(venta.TotalPrecio ?? 0) + (venta.Vuelto ?? 0):F2}").FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            $"{venta.Vuelto ?? 0:F2}").FontSize(8);

                        table.Cell().Background(backgroundColor).Padding(5).Text(
                            $"{venta.TotalPrecio ?? 0:F2}").FontSize(8);

                        contador++;
                    }
                });
            }

            private string ObtenerTipoDocumentoFormateado(string tipoComprobante)
            {
                return tipoComprobante switch
                {
                    "BOLETA" => "BOLETA DE VENTA\nELECTRÓNICA",
                    "FACTURA" => "FACTURA DE VENTA\nELECTRÓNICA",
                    "NOTA DE VENTA" => "NOTA DE VENTA",
                    _ => tipoComprobante
                };
            }
        }
    }


    //Detalle de Ventas PDF
    public static class DetalleVentasPdfHelper
    {
        public static byte[] GenerarDetalleVentas(
            List<DetalleVentaResponse> detallesVenta,
            string vendedor,
            DateTime fechaReporte,
            string? filtroFecha = null)
        {
            var documento = new DocumentoDetalleVentas(detallesVenta, vendedor, fechaReporte, filtroFecha);
            return documento.GeneratePdf();
        }

        private class DocumentoDetalleVentas : IDocument
        {
            private readonly List<DetalleVentaResponse> _detalles;
            private readonly string _vendedor;
            private readonly DateTime _fecha;
            private readonly string? _filtroFecha;

            public DocumentoDetalleVentas(List<DetalleVentaResponse> detalles,
                                          string vendedor,
                                          DateTime fecha,
                                          string? filtroFecha = null)
            {
                _detalles = detalles;
                _vendedor = vendedor;
                _fecha = fecha;
                _filtroFecha = filtroFecha;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontFamily("Helvetica").FontSize(9));

                    page.Content().Column(content =>
                    {
                        // Paso 1: Dibuja el encabezado una sola vez, al principio del contenido.
                        content.Item().Element(ConstruirEncabezado);

                        // Paso 2: Dibuja tu tabla de detalles original.
                        // QuestPDF la dividirá en varias páginas si es necesario,
                        // y el encabezado de la tabla (definido en ConstruirTablaDetalles) sí se repetirá.
                        content.Item().Element(ConstruirTablaDetallesFlexible);

                        // Paso 3: Agrega un espacio y un título para la tabla de resumen.
                        content.Item().PaddingTop(20); // Un espacio para separar las tablas
                        content.Item().Text("Resumen por Producto")
                            .Bold().FontSize(12).FontColor(Colors.Black);
                        content.Item().PaddingTop(5);

                        // Paso 4: Dibuja la nueva tabla de resumen al final de todo.
                        content.Item().Element(ConstruirTablaResumen);
                    });

                });
            }

            private void ConstruirEncabezado(IContainer container)
            {
                container.Column(column =>
                {
                    // Título principal
                    column.Item().Text("Reporte Detalle de Ventas por Productos")
                        .Bold().FontSize(14).FontColor(Colors.Black);

                    column.Item().PaddingTop(10);

                    // Información de la empresa en dos columnas
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(leftColumn =>
                        {
                            leftColumn.Item().Text("Empresa: GRUPO EMPRESARIAL KELPAD SRL").FontSize(10);
                            leftColumn.Item().Text("Ruc: 20613771591").FontSize(10);
                            leftColumn.Item().Text($"Vendedor: {_vendedor}").FontSize(10);
                            leftColumn.Item().Text($"Total productos vendidos: {_detalles.Sum(d => d.Cantidad ?? 0)}").FontSize(10);
                        });

                        row.RelativeItem().Column(rightColumn =>
                        {
                            rightColumn.Item().Text($"Fecha reporte: {_fecha:yyyy-MM-dd}").FontSize(10);
                            rightColumn.Item().Text("Establecimiento: Jr. Calixto 563 - JUNÍN - Huancayo").FontSize(10);
                            if (!string.IsNullOrEmpty(_filtroFecha))
                            {
                                rightColumn.Item().Text($"Período: {_filtroFecha}").FontSize(10);
                            }
                            rightColumn.Item().Text($"Total registros: {_detalles.Count}").FontSize(10);
                        });
                    });

                    column.Item().PaddingTop(10);

                    // Totales generales
                    var totalImporte = _detalles.Sum(d => d.Importe ?? 0);
                    var totalDescuentos = _detalles.Sum(d => (d.Descuento ?? 0) * (d.Cantidad ?? 1));
                    var totalNeto = totalImporte - totalDescuentos;

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Total Bruto: S/ {totalImporte:F2}").FontSize(10).Bold();
                        row.RelativeItem().Text($"Total Descuentos: S/ {totalDescuentos:F2}").FontSize(10).Bold();
                        row.RelativeItem().Text($"Total Neto: S/ {totalNeto:F2}").FontSize(10).Bold();
                    });

                    column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });
            }

            // Alternative approach using RelativeColumn for better flexibility
            private void ConstruirTablaDetallesFlexible(IContainer container)
            {
                container.Table(table =>
                {
                    // Using relative columns for better responsiveness
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(0.5f); // # (smallest)
                        columns.RelativeColumn(1f);   // ID Venta
                        columns.RelativeColumn(3f);   // Producto (largest)
                        columns.RelativeColumn(1f);   // Cantidad
                        columns.RelativeColumn(1.5f); // Precio Unit.
                        columns.RelativeColumn(1.5f); // Descuento
                        columns.RelativeColumn(1.5f); // Importe
                    });

                    // Rest of the table implementation remains the same...
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("#").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("ID Venta").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Producto").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Cantidad").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Precio Unit.").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Descuento").SemiBold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Importe").SemiBold().FontSize(8);
                    });

                    // Data rows...
                    int contador = 1;
                    foreach (var detalle in _detalles)
                    {
                        var backgroundColor = contador % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                        table.Cell().Background(backgroundColor).Padding(3).Text(contador.ToString()).FontSize(8);
                        table.Cell().Background(backgroundColor).Padding(3).Text(detalle.IdVentas?.ToString() ?? "").FontSize(8);
                        table.Cell().Background(backgroundColor).Padding(3).Text(detalle.NombreProducto ?? "").FontSize(8);
                        table.Cell().Background(backgroundColor).Padding(3).Text($"{detalle.Cantidad ?? 0}").FontSize(8);
                        table.Cell().Background(backgroundColor).Padding(3).Text($"S/ {detalle.PrecioUnit ?? 0:F2}").FontSize(8);
                        table.Cell().Background(backgroundColor).Padding(3).Text($"S/ {(detalle.Descuento ?? 0) :F2}").FontSize(8);
                        table.Cell().Background(backgroundColor).Padding(3).Text($"S/ {detalle.Importe ?? 0:F2}").FontSize(8);

                        contador++;
                    }
                });

            }
            // MÉTODO NUEVO: Se añade la tabla de resumen
            private void ConstruirTablaResumen(IContainer container)
            {
                // Agrupar y sumar los detalles por producto
                var resumenPorProducto = _detalles
                    .GroupBy(d => d.NombreProducto)
                    .Select(g => new
                    {
                        Producto = g.Key,
                        CantidadTotal = g.Sum(item => item.Cantidad ?? 0),
                        ImporteTotal = g.Sum(item => item.Importe ?? 0)
                    })
                    .OrderBy(r => r.Producto)
                    .ToList();

                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(265); // Producto
                        columns.ConstantColumn(120); // Cantidad Total
                        columns.ConstantColumn(120); // Importe Total
                    });

                    // Encabezado de la tabla de resumen
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Producto").Bold().FontSize(8);
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Cantidad Total").Bold().FontSize(8).AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Importe Total").Bold().FontSize(8).AlignRight();
                    });

                    // Filas con los datos del resumen
                    foreach (var item in resumenPorProducto)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(item.Producto ?? "Sin Nombre").FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text($"{item.CantidadTotal}").FontSize(8).AlignRight();
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text($"S/ {item.ImporteTotal:F2}").FontSize(8).AlignRight();
                    }
                });
            }
        }
    }
}
