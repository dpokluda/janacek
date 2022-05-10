using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceA.Microservices;

namespace ServiceA
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();
            foreach ((string key, object value) in app.Properties)
            {
                Debug.WriteLine($"Properties[\"{key}\"] = \"{value.ToString()}\"");
            }

            app.UseJanacek()
               .Add("role: math, cmd: sum", MathService.OnSum)
               .Add("role: math, cmd: product", MathService.OnProduct)
               .Listen("http://localhost:65473/janacek/act/");
        }
    }
}
