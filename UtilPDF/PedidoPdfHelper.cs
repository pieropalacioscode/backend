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
}
