using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SignalR.API.Hubs;
using SignalR.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(opt=> {//Sonradan eklendi
                opt.UseSqlServer(Configuration["ConStr"]);
            });

            services.AddCors(opt=> {
                opt.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:44365").AllowAnyHeader().AllowAnyMethod().AllowCredentials();//SignalR.Web için eklendi ve  SignalR.Web urlim
                });
            });

            services.AddControllers();
            services.AddSignalR();//Signal kullanýlacaðýný belirtiyoruz
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SignalR.API", Version = "v1" });
            });
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SignalR.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");//SignalR.Web için eklendi

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MyHub>("/MyHub");//Sonradan ekledik. MyHub clasýmýzý burdan tanýmladýk ve Sitemiz/MyHub ile baðlanabiliriz
            });
        }
    }
}
