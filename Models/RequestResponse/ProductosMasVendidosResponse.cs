﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class ProductosMasVendidosResponse
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int TotalVendidos { get; set; }
    }
}
