using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using FreeSqlDemo.Infrastructure.JWTOptions;
using FreeSqlDemo.Infrastructure.RepositoryBase;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using static FreeSqlDemo.Infrastructure.DI.ICollectionServiceExtensions;

namespace FreeSqlDemo.Infrastructure.DomainBase
{
    /// <summary>
    /// 领域基类
    /// </summary>
    public abstract class DomainBase : IDomainBase
    {
        public ILogger Logger { get; }
        public IRepositoryUnitOfWork UnitOfWork { get; }
        public CurrentUser CurrentUser { get; }
        public IMapper Mapper { get; }

        protected DomainBase(IServiceProvider service, ILogger logger)
        {
            Logger = logger;
            UnitOfWork = service.GetRequiredService<IRepositoryUnitOfWork>();
            CurrentUser = GetCurrentUser(service.GetService<IHttpContextAccessor>().HttpContext?.User, service);
            Mapper = service.GetRequiredService<IMapper>();

        }
        public CurrentUser GetCurrentUser(ClaimsPrincipal claimsPrincipal, IServiceProvider service)
        {
            var claims = claimsPrincipal.Claims;
            var userid = claims.FirstOrDefault(x => x.Type == ClaimsType.UserId)?.Value;
            var user = service.GetService<IFreeSql>().Cache.Get<CurrentUser>($"{CacheKey.Login}:{userid}");
            return user;
        }
    }
}
