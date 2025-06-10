﻿using DBModel.DB;
using IRepository;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PedidoProveedorRepository : GenericRepository<PedidoProveedor>, IPedidoProveedorRepository
    {
        public List<PedidoProveedor> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }
    }
}
