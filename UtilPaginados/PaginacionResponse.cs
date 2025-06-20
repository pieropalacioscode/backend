﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPaginados
{
    public class PaginacionResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
