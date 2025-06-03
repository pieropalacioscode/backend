﻿using DBModel.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class DatalleCarrito
    {
        public  List<Carrito> Items {get;set;}
        public decimal TotalAmount { get;set;}
        public PersonaRequest Persona{ get; set; }
        public string tipoComprobante { get; set; }
        public string tipoPago { get; set; }
    }
}
