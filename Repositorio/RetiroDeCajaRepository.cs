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
    public class RetiroDeCajaRepository : GenericRepository<RetiroDeCaja>, IRetiroDeCajaRepository
    {
        public List<RetiroDeCaja> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }
    }
}
