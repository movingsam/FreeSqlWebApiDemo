using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSqlDemo.Infrastructure.Entity.Page;
using Microsoft.AspNetCore.Mvc;

namespace FreeSqlDemo.Infrastructure.MVC
{
    public abstract class DemoControllerBase : ControllerBase
    {
        public DemoControllerBase(IServiceProvider service)
        {

        }
        /// <summary>
        /// 定义一个分页返回模板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageData"></param>
        /// <returns></returns>
        public OkObjectResult Ok<T>(PageListBase<T> pageData) where T : class
        {
            return base.Ok(new
            {
                Data = pageData,
                PageNumber = pageData.PageNumber,
                PageSize = pageData.PageSize,
                Total = pageData.Total
            });
        }
    }
}
