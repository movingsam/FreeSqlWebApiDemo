using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSqlDemo.Infrastructure.Entity.Page
{
    public class PageListBase<T> : List<T>
    {
        public PageListBase()
        {

        }

        public PageListBase(IEnumerable<T> data, long total, int pageNumber = 0, int pageSize = 10) : base(data)
        {
            Total = total;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public long Total { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
