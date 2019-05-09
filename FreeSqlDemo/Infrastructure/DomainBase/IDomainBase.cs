using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.Extensions.Logging;

namespace FreeSqlDemo.Infrastructure.DomainBase
{
    public interface IDomainBase
    {
        ILogger Logger { get; }
        IRepositoryUnitOfWork UnitOfWork { get; }
        CurrentUser CurrentUser { get; }
    }
}
