using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
using Supermarket.API.Context;
using Supermarket.API.Dtos.Mappings;
using Supermarket.API.Extensions;
using Supermarket.API.Filters;
using Supermarket.API.Logging;
using Supermarket.API.Models;
using Supermarket.API.Repository;

namespace Supermarket.API
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
            string mySqlConnection = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<AppDbContext>(options =>
            {
                //options.UseSqlite(Configuration.GetConnectionString("DevConnection")); // Sqlite // old Pomelo versions
                options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));   // get Connection with new Pomelo
            });

            services.AddControllers();
            // using System.Text.Json -> doesn't have a reference looping treatment for now, using JsonIgnoreAttribute to handle this
            //services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Supermarket.API", Version = "v1" }));

            // filter with custom logging service
            services.AddScoped<ApiLoggingFilter>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // AutoMapper - register and condig
            var mappingConfig = new MapperConfiguration(mc =>
                mc.AddProfile(new MappingProfile()));

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supermarket.API v1"));
            }
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // add middleware to handle exceptions
            app.ConfigureExceptionHandler();

            // defining multiple loggers level and color
            loggerFactory.AddColoredConsoleLogger();
            loggerFactory.AddColoredConsoleLogger(c =>
            {
                c.LogLevel = LogLevel.Debug;
                c.Color = ConsoleColor.Gray;
                c.IsWritingToFile = false;
            });
            loggerFactory.AddColoredConsoleLogger(c =>
            {
                c.LogLevel = LogLevel.Information;
                c.Color = ConsoleColor.Blue;
                c.IsWritingToFile = true;
            });
        }
    }
}
