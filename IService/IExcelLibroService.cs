using Microsoft.AspNetCore.Http;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IExcelLibroService
    {
        Task<List<LibroExcelRequest>> LeerExcelLibros(IFormFile archivoExcel);
    }
}
