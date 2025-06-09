using DBModel.DB;
using Models.RequestResponse;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Net.Http;

namespace UtilPDF
{
    public class PdfGenerator
    {
        // Función auxiliar para medir el ancho del texto
        public static double MeasureTextWidth(XGraphics gfx, string text, XFont font)
        {
            XSize size = gfx.MeasureString(text, font);
            return size.Width;
        }

        public static MemoryStream CreateDetalleVentaPdf(List<DetalleVentaRequest> detalles, Venta venta, Persona persona, string logoUrl = "https://firebasestorage.googleapis.com/v0/b/ecomercesa-3c1ff.appspot.com/o/ic_launcher.png?alt=media&token=65889eaa-978d-4264-8ee2-e005e568a1e5")
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            // Tamaño optimizado para ticket
            page.Width = XUnit.FromMillimeter(80);
            page.Height = XUnit.FromMillimeter(300 + (detalles.Count * 10));

            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Establecer fuentes
            XFont titleFont = new XFont("Arial", 12, XFontStyle.Bold);
            XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);
            XFont normalFont = new XFont("Arial", 8);
            XFont smallFont = new XFont("Arial", 7);

            double currentY = 10;
            double marginLeft = 5;
            double pageWidth = page.Width - marginLeft * 2;

            // Cargar y dibujar logo real desde URL
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = client.GetByteArrayAsync(logoUrl).Result;
                    XImage logo = XImage.FromStream(() => new MemoryStream(imageBytes));
                    double logoSize = 50;
                    double logoX = marginLeft + pageWidth / 2 - logoSize / 2;
                    gfx.DrawImage(logo, logoX, currentY, logoSize, logoSize);
                    currentY += logoSize + 10;
                }
            }
            catch
            {
                // Si falla la carga del logo, usar el diseño circular como fallback
                XBrush greenBrush = new XSolidBrush(XColor.FromArgb(102, 204, 0));
                XBrush blueBrush = new XSolidBrush(XColor.FromArgb(0, 100, 200));

                double logoSize = 50;
                double logoX = marginLeft + pageWidth / 2 - logoSize / 2;
                gfx.DrawEllipse(greenBrush, logoX, currentY, logoSize, logoSize);

                gfx.DrawString("Librería", new XFont("Arial", 10, XFontStyle.Bold), blueBrush,
                    new XRect(logoX, currentY + 12, logoSize, 12), XStringFormats.TopCenter);
                gfx.DrawString("Saber", new XFont("Arial", 10, XFontStyle.Bold), blueBrush,
                    new XRect(logoX, currentY + 25, logoSize, 12), XStringFormats.TopCenter);

                currentY += logoSize + 10;
            }

            // Información de la empresa centrada y más compacta
            gfx.DrawString("GRUPO EMPRESARIAL KELPAD SRL", headerFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 10), XStringFormats.TopCenter);
            currentY += 12;

            gfx.DrawString("RUC 20613771591", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("Jr. Calixto 563 , Huancayo , Huancayo , Junín", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("D. Comercial: Jr. Calixto 563", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("Central telefónica: 933337735", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("Email: ventas@libreriasaber.com", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("Web: libreriasaber.com", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("Venta de Libros, diccionarios, folletos, agendas, etc.", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 15;

            // Título del comprobante según validación
            string tituloComprobante = "";
            string prefijoComprobante = "";

            switch (venta.TipoComprobante?.ToUpper())
            {
                case "BOLETA":
                    tituloComprobante = "Boleta de Venta Electrónica";
                    prefijoComprobante = "B";
                    break;
                case "FACTURA":
                    tituloComprobante = "Factura Electrónica";
                    prefijoComprobante = "FTR1-";
                    break;
                case "NOTA":
                    tituloComprobante = "Nota de Venta";
                    prefijoComprobante = "N";
                    break;
                default:
                    tituloComprobante = "Comprobante";
                    prefijoComprobante = "C";
                    break;
            }

            gfx.DrawString(tituloComprobante, titleFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 12), XStringFormats.TopCenter);
            currentY += 15;

            gfx.DrawString($"{prefijoComprobante}{venta.NroComprobante ?? "00000013"}", titleFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 12), XStringFormats.TopCenter);
            currentY += 20;

            // Información de emisión y cliente - formato mejorado
            string fechaEmision = venta.FechaVenta?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
            string horaEmision = DateTime.Now.ToString("HH:mm:ss");

            gfx.DrawString($"F. Emisión:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString(fechaEmision, normalFont, XBrushes.Black, new XPoint(marginLeft + 45, currentY));
            currentY += 10;

            gfx.DrawString($"H. Emisión:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString(horaEmision, normalFont, XBrushes.Black, new XPoint(marginLeft + 45, currentY));
            currentY += 10;

            gfx.DrawString($"F. Vencimiento:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString(fechaEmision, normalFont, XBrushes.Black, new XPoint(marginLeft + 55, currentY));
            currentY += 10;

            gfx.DrawString($"Cliente:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString(persona.Nombre ?? "CONSUMIDOR FINAL", normalFont, XBrushes.Black, new XPoint(marginLeft + 30, currentY));
            currentY += 10;

            if (venta.TipoComprobante?.ToUpper() == "FACTURA")
            {
                gfx.DrawString($"RUC:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
                gfx.DrawString(persona.NumeroDocumento ?? "--------", normalFont, XBrushes.Black, new XPoint(marginLeft + 25, currentY));
                currentY += 10;
            }
            else
            {
                gfx.DrawString($"DNI:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
                gfx.DrawString(persona.NumeroDocumento ?? "--------", normalFont, XBrushes.Black, new XPoint(marginLeft + 25, currentY));
                currentY += 10;
            }

            // Dirección en líneas separadas
            gfx.DrawString("Dirección:", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 10;
            gfx.DrawString("Cal. Chimu Otr. Barrio Residencial", smallFont, XBrushes.Black, new XPoint(marginLeft + 5, currentY));
            currentY += 8;
            gfx.DrawString("Descam Mz. F Lt. 3 , Ate , Lima , Lima", smallFont, XBrushes.Black, new XPoint(marginLeft + 5, currentY));
            currentY += 15;

            // Tabla de productos mejorada
            double tableY = currentY;
            double tableWidth = pageWidth;
            double headerHeight = 12;

            // Encabezado de tabla con fondo gris
            XBrush lightGrayBrush = new XSolidBrush(XColor.FromArgb(240, 240, 240));
            gfx.DrawRectangle(XPens.Black, lightGrayBrush, marginLeft, tableY, tableWidth, headerHeight);

            // Anchos de columnas mejorados y balanceados
            double cantWidth = 12;
            double unidadWidth = 18;
            double descripcionWidth = pageWidth - cantWidth - unidadWidth - 22 - 20; // Ajustado para mejor balance
            double punitWidth = 22;
            double totalWidth = 20;

            double xPos = marginLeft + 1;

            // Headers con mejor alineación
            gfx.DrawString("Cant", smallFont, XBrushes.Black,
                new XRect(xPos, tableY + 1, cantWidth, headerHeight), XStringFormats.TopCenter);
            xPos += cantWidth;

            gfx.DrawString("Unid", smallFont, XBrushes.Black,
                new XRect(xPos, tableY + 1, unidadWidth, headerHeight), XStringFormats.TopCenter);
            xPos += unidadWidth;

            gfx.DrawString("Descripción", smallFont, XBrushes.Black,
                new XRect(xPos, tableY + 1, descripcionWidth, headerHeight), XStringFormats.TopLeft);
            xPos += descripcionWidth;

            gfx.DrawString("P.Unit", smallFont, XBrushes.Black,
                new XRect(xPos, tableY + 1, punitWidth, headerHeight), XStringFormats.TopCenter);
            xPos += punitWidth;

            gfx.DrawString("Total", smallFont, XBrushes.Black,
                new XRect(xPos, tableY + 1, totalWidth, headerHeight), XStringFormats.TopCenter);

            currentY = tableY + headerHeight;

            // Líneas verticales de la tabla más precisas
            double tableEndY = currentY + (detalles.Count * 16) + 3;

            // Líneas verticales
            double line1 = marginLeft;
            double line2 = marginLeft + cantWidth;
            double line3 = marginLeft + cantWidth + unidadWidth;
            double line4 = marginLeft + cantWidth + unidadWidth + descripcionWidth;
            double line5 = marginLeft + cantWidth + unidadWidth + descripcionWidth + punitWidth;
            double line6 = marginLeft + tableWidth;

            gfx.DrawLine(XPens.Black, line1, tableY, line1, tableEndY);
            gfx.DrawLine(XPens.Black, line2, tableY, line2, tableEndY);
            gfx.DrawLine(XPens.Black, line3, tableY, line3, tableEndY);
            gfx.DrawLine(XPens.Black, line4, tableY, line4, tableEndY);
            gfx.DrawLine(XPens.Black, line5, tableY, line5, tableEndY);
            gfx.DrawLine(XPens.Black, line6, tableY, line6, tableEndY);

            // Productos con mejor formato
            decimal subtotalGeneral = 0m;
            foreach (var detalle in detalles)
            {
                double rowHeight = 16;

                // Línea horizontal superior de cada fila
                gfx.DrawLine(XPens.Black, marginLeft, currentY, marginLeft + tableWidth, currentY);

                // Contenido de cada celda con mejor alineación
                xPos = line1 + 1;
                gfx.DrawString((detalle.Cantidad ?? 1).ToString(), smallFont, XBrushes.Black,
                    new XRect(xPos, currentY + 2, cantWidth - 2, rowHeight), XStringFormats.TopCenter);

                xPos = line2 + 1;
                gfx.DrawString("UND", smallFont, XBrushes.Black,
                    new XRect(xPos, currentY + 2, unidadWidth - 2, rowHeight), XStringFormats.TopCenter);

                xPos = line3 + 2;
                string nombreProducto = detalle.NombreProducto ?? "Producto";
                if (nombreProducto.Length > 18)
                    nombreProducto = nombreProducto.Substring(0, 15) + "...";
                gfx.DrawString(nombreProducto, smallFont, XBrushes.Black,
                    new XRect(xPos, currentY + 2, descripcionWidth - 4, rowHeight), XStringFormats.TopLeft);

                xPos = line4 + 1;
                gfx.DrawString($"{(detalle.PrecioUnit ?? 0):F2}", smallFont, XBrushes.Black,
                    new XRect(xPos, currentY + 2, punitWidth - 2, rowHeight), XStringFormats.TopCenter);

                xPos = line5 + 1;
                decimal subtotalProducto = (detalle.Cantidad ?? 1) * (detalle.PrecioUnit ?? 0);
                gfx.DrawString($"{subtotalProducto:F2}", smallFont, XBrushes.Black,
                    new XRect(xPos, currentY + 2, totalWidth - 2, rowHeight), XStringFormats.TopCenter);

                subtotalGeneral += subtotalProducto;
                currentY += rowHeight;
            }

            // Línea final de la tabla
            gfx.DrawLine(XPens.Black, marginLeft, currentY, marginLeft + tableWidth, currentY);
            currentY += 12;

            // Totales con descuento incluido
            double totalStartX = marginLeft + pageWidth - 35;

            // Subtotal antes de descuento
            gfx.DrawString($"Subtotal: S/", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString($"{subtotalGeneral:F2}", normalFont, XBrushes.Black, new XPoint(totalStartX, currentY));
            currentY += 10;

            // Descuento (si existe)
            foreach (var detalle in detalles)
            {
                if (detalle.Descuento > 0)
                {
                    gfx.DrawString($"Descuento: S/", normalFont, XBrushes.Red, new XPoint(marginLeft, currentY));
                    gfx.DrawString($"-{detalle.Descuento:F2}", normalFont, XBrushes.Red, new XPoint(totalStartX, currentY));
                    currentY += 10;
                }

            }
            decimal descuentoTotal = detalles.Sum(d => d.Descuento ?? 0); // Suma todos los descuentos
            decimal totalFinal = subtotalGeneral - descuentoTotal;


            // Calcular total final después de descuento


            gfx.DrawString($"Op. Exoneradas: S/", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString($"{totalFinal:F2}", normalFont, XBrushes.Black, new XPoint(totalStartX, currentY));
            currentY += 10;

            gfx.DrawString("IGV: S/", normalFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString("0.00", normalFont, XBrushes.Black, new XPoint(totalStartX, currentY));
            currentY += 10;

            gfx.DrawString($"Total a pagar: S/", headerFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            gfx.DrawString($"{totalFinal:F2}", headerFont, XBrushes.Black, new XPoint(totalStartX, currentY));
            currentY += 15;

            // Separador
            gfx.DrawLine(XPens.Black, marginLeft, currentY, marginLeft + pageWidth, currentY);
            currentY += 10;

            // Información adicional con mejor formato
            gfx.DrawString($"Son: {NumeroALetras(totalFinal)} Soles", normalFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopLeft);
            currentY += 12;

            // Información bancaria centrada
            gfx.DrawString("YAPE/PLIN Soles N°: 933337735", smallFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("INTERBANK Soles N°: 50030069270560", smallFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 10;

            gfx.DrawString("CCI: 003500030069270560569", smallFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 12;

            // Código hash
            gfx.DrawString("Código hash:", smallFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 8;
            gfx.DrawString("ACV1wIESOOswKKp6xvC3Fdos=", smallFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 10;

            // Condición de pago y pagos
            gfx.DrawString("Condición de Pago: Contado", smallFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 8;

            gfx.DrawString("Pagos:", smallFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 8;

            gfx.DrawString($"- Efectivo : S/ {totalFinal:F2}", smallFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 8;

            gfx.DrawString("Vendedor: ADMIN", smallFont, XBrushes.Black, new XPoint(marginLeft, currentY));
            currentY += 15;

            // Mensaje promocional centrado
            gfx.DrawString("Compra en libreriasaber.com Hacemos envíos a todo el Perú", smallFont, XBrushes.Black,
                new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            currentY += 15;

            // Representación impresa (solo para comprobantes electrónicos)
            if (venta.TipoComprobante?.ToUpper() != "NOTA")
            {
                gfx.DrawString($"Representación impresa de la {tituloComprobante.ToUpper()}", smallFont, XBrushes.Black,
                    new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
                currentY += 8;

                gfx.DrawString("Esta puede ser consultada en", smallFont, XBrushes.Black,
                    new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
                currentY += 8;

                gfx.DrawString("https://libreriasaber.facturaperu.com/buscar", smallFont, XBrushes.Blue,
                    new XRect(marginLeft, currentY, pageWidth, 8), XStringFormats.TopCenter);
            }

            // Guardar el PDF en un MemoryStream y retornar
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            return stream;
        }

        // Función auxiliar para convertir números a letras
        private static string NumeroALetras(decimal numero)
        {
            if (numero == 0) return "Cero";

            int parteEntera = (int)Math.Floor(numero);
            int centavos = (int)((numero - parteEntera) * 100);

            return $"{ConvertirEnteroALetras(parteEntera)} con {centavos:00}/100";
        }

        private static string ConvertirEnteroALetras(int numero)
        {
            if (numero == 0) return "Cero";

            string[] unidades = { "", "Uno", "Dos", "Tres", "Cuatro", "Cinco", "Seis", "Siete", "Ocho", "Nueve" };
            string[] decenas = { "", "", "Veinte", "Treinta", "Cuarenta", "Cincuenta", "Sesenta", "Setenta", "Ochenta", "Noventa" };
            string[] especiales = { "Diez", "Once", "Doce", "Trece", "Catorce", "Quince", "Dieciséis", "Diecisiete", "Dieciocho", "Diecinueve" };

            if (numero < 10) return unidades[numero];
            if (numero >= 10 && numero < 20) return especiales[numero - 10];
            if (numero < 100) return decenas[numero / 10] + (numero % 10 > 0 ? " y " + unidades[numero % 10] : "");

            // Para números mayores, implementación básica
            return numero.ToString();
        }
    }
}