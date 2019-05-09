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
using Microsoft.Extensions.Options;

namespace FreeSqlDemo.Infrastructure.DI
{
    public static class ICollectionServiceExtensions
    {
        public static void AddFreeSql(this IServiceCollection service)
        {
            //获取配置
            //IConfiguration config = new ConfigurationBuilder()
            //    .AddJsonFile($"appsettings.json")
            //    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
            //    .Build();
            var freeSql = service.BuildServiceProvider().GetRequiredService<IOptions<FreeSqlConfig>>().Value;
            //注入FreeSql
            service.AddSingleton<IFreeSql>(f =>
            {
                var freeBuilder = new FreeSqlBuilder()
                    .UseAutoSyncStructure(true)
                    .UseConnectionString(freeSql.DataType, freeSql.MasterConnetion)
                    .UseLazyLoading(true)
                    .UseCache(null);
                if (freeSql.SlaveConnections?.Count > 0)//判断是否存在从库
                {
                    freeBuilder.UseSlave(freeSql.SlaveConnections.Select(x => x.ConnectionString).ToArray());
                }
                return freeBuilder.Build();
            });
            //注入Uow
            service.AddScoped<IRepositoryUnitOfWork>(f => f.GetRequiredService<IFreeSql>().CreateUnitOfWork());
            service.AddHttpContextAccessor();
            service.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //service.AddScoped<CurrentUser>(f => f.GetRequiredService<IHttpContextAccessor>().HttpContext?.User?.GetCurrentUser(f));
            service.AddServer(typeof(IRepKey));
            service.AddServer(typeof(IDomainBase));
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        private static void AddServer(this IServiceCollection service, Type type)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var referencedAssemblies = entryAssembly.GetReferencedAssemblies().Select(Assembly.Load);
            var assemblies = new List<Assembly> { entryAssembly }.Concat(referencedAssemblies);
            service.Scan(s =>
            {
                s.FromAssemblies(entryAssembly).AddClasses(classes => classes.AssignableTo(type))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });
        }

     
    }
}
