using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSqlDemo.Infrastructure.DI;
using FreeSqlDemo.Infrastructure.JWTOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace FreeSqlDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.Configure<FreeSqlConfig>(Configuration.GetSection("FreeSqlConfig"));
            var jwt = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = jwt.ValidateIssuer, //是否验证Issuer
                        ValidateAudience = jwt.ValidateAudience, //是否验证Audience
                        ValidateLifetime = jwt.ValidateLifetime, //是否验证失效时间
                        ValidateIssuerSigningKey = jwt.ValidateIssuerSigningKey, //是否验证SecurityKey
                        ValidAudience = jwt.ValidAudience, //Audience
                        ValidIssuer = jwt.ValidIssuer, //Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.IssuerSigningKey)) //拿到SecurityKey
                    };
                });


            services.AddFreeSql();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "FreeSqlDemo",
                    Description = "RESTful API for FreeSqlDemo",
                    TermsOfService = "None",
                    Contact = null
                });
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "权限认证(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey",
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() }
                });
                var xmlPath = FindAllXmlFile.GetFile(AppContext.BaseDirectory, "*.xml");
                foreach (var item in xmlPath)
                {
                    options.IncludeXmlComments(item, true);
                }
                //options.OperationFilter<HttpHeaderOperation>();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //权限验证开启
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger(
                options =>
                {
                    options.RouteTemplate = "/{documentName}/swagger.json";
                }
            );
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/v1/swagger.json", "FreeSql Demo");
            });
        }
        public class FindAllXmlFile
        {
            public static string[] GetFile(string path, string searchPattern)
            {
                string[] filenames = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
                return filenames;
            }
        }
    }
}
