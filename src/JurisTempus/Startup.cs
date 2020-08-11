using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.AspNetCore;
using JurisTempus.Data;
using JurisTempus.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JurisTempus
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
            services.AddDbContext<BillingContext>(cfg =>
            {
                //cfg.UseSqlServer(Configuration.GetConnectionString("JurisDb"));
                cfg.UseInMemoryDatabase("JurisDb");

            });

            //PER AGGIUNGERE FluentValidation PER API
            services.AddControllers()
              .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddRazorPages()
            //PER AGGIUGNERE FluentValidation PER RAZOR PAGES
            //.AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Validators.CaseValidator>())
            ;
            services.AddControllersWithViews()
            //PER AGGIUNGERE FluentValidation PER MVC CONTROLLER
            //.AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()))
              .AddNewtonsoftJson(cfg => cfg.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            // NOTE: this must go at the end of Configure to SEED INMEMORY DB WITH EF
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<BillingContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}
