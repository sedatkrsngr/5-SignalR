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
using SignalRCovidChart.API.Hubs;
using SignalRCovidChart.API.Models;
using SignalRCovidChart.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRCovidChart.API
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
            services.AddDbContext<CovidDbContext>(options=> {

                options.UseSqlServer(Configuration["ConStr"]);
            });//Veritabaný baðlatýsý

            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:44331").AllowAnyHeader().AllowAnyMethod().AllowCredentials();////SignalRCovidChart.Web için eklendi ve  SignalRCovidChart.Web urlim
                });
            });
            services.AddSignalR();//
            services.AddScoped<CovidService>();//Bir constructorda kullandýðýmýzda nesne örneði üretsin diye ekledik

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SignalRCovidChart.API", Version = "v1" });
            });
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SignalRCovidChart.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");//SignalRCovidChart.Web için eklendi
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {               
                endpoints.MapControllers();
                endpoints.MapHub<CovidHub>("/CovidHub");//
            });
        }
    }
}
