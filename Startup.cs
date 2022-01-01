using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
namespace api
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
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
            services.AddControllers();             
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();        
            string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration["PostgreSql:DATABASE_URL"];            
            Uri databaseUri = null;
            string[] userInfo = {null, null};
            if (databaseUrl != null){
                databaseUri = new Uri(databaseUrl);                
                if (databaseUri != null)
                    userInfo = databaseUri.UserInfo.Split(':');
            }           
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri?.Host ?? Configuration["PostgreSql:Host"],
                Port = int.Parse(databaseUri?.Port.ToString() ?? Configuration["PostgreSql:Port"]),
                Username = userInfo[0] ?? Configuration["PostgreSql:User"],
                Password = userInfo[1] ?? Configuration["PostgreSql:ServerPassword"],
                Database = databaseUri.LocalPath.TrimStart('/') ?? Configuration["PostgreSql:DatabaseName"]
            };
            // string herokuConnectionString = $@"
            //     Server={Configuration["PostgreSql:Host"]};
            //     Port={Configuration["PostgreSql:Port"]};
            //     User Id={Configuration["PostgreSql:User"]};
            //     Password={Configuration["PostgreSql:ServerPassword"]};
            //     Database={Configuration["PostgreSql:DatabaseName"]};
            //     SSL Mode=Require;Trust Server Certificate=true";        
            // NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(herokuConnectionString);                       
            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(builder.ConnectionString));        
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
            });            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
            }                      
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
