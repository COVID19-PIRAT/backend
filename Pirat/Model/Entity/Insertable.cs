﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.DatabaseContext;

namespace Pirat.Model.Entity
{
    public interface Insertable
    {
        /// <summary>
        /// After calling the Add method in context with the insertable, the insertable will have an unique ID given by our database
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Insertable Insert(DemandContext context);
    }
}