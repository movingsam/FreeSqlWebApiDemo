using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.DomainBase;
using FreeSqlDemo.Infrastructure.Entity;
using FreeSqlDemo.Infrastructure.JWTOptions;
using FreeSqlDemo.Infrastructure.Repository;
using FreeSqlDemo.Infrastructure.RepositoryBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FreeSqlDemo.Infrastructure.DI
{
    public static class ICollectionServiceExtensions
    {
        public static void AddFreeSql(this IServiceCollection service)
        {
            var freeSql = service.BuildServiceProvider().GetRequiredService<IOptions<FreeSqlConfig>>().Value;
            //注入FreeSql
            service.AddSingleton<IFreeSql>(f =>
            {
                var log = f.GetRequiredService<ILogger<IFreeSql>>();
                var freeBuilder = new FreeSqlBuilder()
                    .UseAutoSyncStructure(true)
                    .UseConnectionString(freeSql.DataType, freeSql.MasterConnetion)
                    .UseLazyLoading(true)
                    .UseMonitorCommand(
                           executing =>
                       {
                           //执行中打印日志
                           log.LogInformation(executing.CommandText);
                       }
                        //   null,
                        //(executed, t) =>
                        //{
                        //    //执行后打印日志
                        //    log.LogWarning(t);
                        //}
                        )
                    .UseCache(null);
                if (freeSql.SlaveConnections?.Count > 0)//判断是否存在从库
                {
                    freeBuilder.UseSlave(freeSql.SlaveConnections.Select(x => x.ConnectionString).ToArray());
                }
                var freesql = freeBuilder.Build();
                //我这里禁用了导航属性联级插入的功能
                freesql.SetDbContextOptions(opt => opt.EnableAddOrUpdateNavigateList = false);
                return freesql;
            });
            //注入Uow
            service.AddScoped<IRepositoryUnitOfWork>(f => f.GetRequiredService<IFreeSql>().CreateUnitOfWork());
            //注入HttpContextAccessor 可以从IOC中拿到HttpContext的内容
            service.AddHttpContextAccessor();
            service.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //注入当前用户 利用了缓存查询当前用户信息
            service.AddScoped<CurrentUser>(f => f.GetRequiredService<IHttpContextAccessor>().HttpContext?.User?.GetCurrentUser(f));
            //使用类库Scrutor做发现注入类库和服务
            service.AddServer(typeof(IRepKey));
            service.AddServer(typeof(IDomainBase));
            //Automapper和配置模板 注入
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
        /// <summary>
        /// Scrutor注入使用
        /// </summary>
        /// <param name="service"></param>
        /// <param name="type"></param>
        private static void AddServer(this IServiceCollection service, Type type)
        {
            //程序入口程序集
            var entryAssembly = Assembly.GetEntryAssembly();
            //再获取程序入口程序集的引用程序集
            var referencedAssemblies = entryAssembly.GetReferencedAssemblies().Select(Assembly.Load);
            //合并这些程序集
            var assemblies = new List<Assembly> { entryAssembly }.Concat(referencedAssemblies);
            service.Scan(s =>
            {
                //扫描所有程序集通过接口找相关实现
                s.FromAssemblies(assemblies).AddClasses(classes => classes.AssignableTo(type))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                    ;//这里使用了Scope周期 可以使用 WithLifetime(ServiceLifetime.Singleton)传入其他生命周期来灵活配置

            });
        }
        /// <summary>
        /// 获取当前用户信息的方法
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static CurrentUser GetCurrentUser(this ClaimsPrincipal claimsPrincipal, IServiceProvider service)
        {
            //先从context中拿到claims
            var claims = claimsPrincipal.Claims;
            //再解析userid类型拿到userid
            var userid = claims.FirstOrDefault(x => x.Type == ClaimsType.UserId)?.Value;
            //从FreeSql提供的Cache(其实就是内存缓存)中拿到登陆人的信息
            //我这里的规则是用户登陆时会存入内存缓存Key:Login:{UserId} 若查不到说明此人的授权期限到期了
            //因为我设置的缓存时间和授权过期时间是相同的
            var user = service.GetService<IFreeSql>().Cache.Get<CurrentUser>($"{CacheKey.Login}:{userid}");
            return user;
        }

    }
}
