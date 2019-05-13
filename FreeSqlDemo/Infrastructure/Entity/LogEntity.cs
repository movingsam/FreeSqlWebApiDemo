using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSqlDemo.Infrastructure.Entity
{
    public class LogEntity : EntityBase<int>
    {
        public string Action { get; set; }
    }
}
