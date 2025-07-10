using IService;
using Microsoft.AspNetCore.Http;
using Models.RequestResponse;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ExcelLibroService : IExcelLibroService
    {
        public ExcelLibroService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Piero Palacios"); // IMPORTANTE
        }


        public async Task<List<LibroExcelRequest>> LeerExcelLibros(IFormFile archivoExcel)
        {
            var listaLibros = new List<LibroExcelRequest>();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Primera hoja
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Ignora encabezado
                    {
                        var libro = new LibroRequest
                        {
                            Titulo = worksheet.Cells[row, 1].Text,
                            Isbn = worksheet.Cells[row, 2].Text,
                            Tamanno = worksheet.Cells[row, 3].Text,
                            Descripcion = worksheet.Cells[row, 4].Text,
                            Condicion = worksheet.Cells[row, 5].Text,
                            Impresion = worksheet.Cells[row, 6].Text,
                            TipoTapa = worksheet.Cells[row, 7].Text,
                            Estado = bool.TryParse(worksheet.Cells[row, 8].Text, out bool estado) ? estado : true,
                            IdSubcategoria = int.TryParse(worksheet.Cells[row, 9].Text, out var subcat) ? subcat : 0,
                            IdTipoPapel = int.TryParse(worksheet.Cells[row, 10].Text, out var papel) ? papel : 0,
                            IdProveedor = int.TryParse(worksheet.Cells[row, 11].Text, out var prov) ? prov : 0
                        };

                        var autor = new AutorRequest
                        {
                            Nombre = worksheet.Cells[row, 12].Text,
                            Apellido = worksheet.Cells[row, 13].Text,
                            Descripcion = worksheet.Cells[row, 14].Text
                        };

                        var precioVenta = decimal.TryParse(worksheet.Cells[row, 15].Text, out var precio) ? precio : 0;
                        var stock = int.TryParse(worksheet.Cells[row, 16].Text, out var stk) ? stk : 0;

                        listaLibros.Add(new LibroExcelRequest
                        {
                            Libro = libro,
                            Autor = autor,
                            PrecioVenta = precioVenta,
                            Stock = stock
                        });
                    }
                }
            }

            return listaLibros;
        }

    }
}
