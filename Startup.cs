using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using OpenTracing.Util;

namespace opentracing_demo {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);

            services.AddLogging ();

            if (Convert.ToBoolean (Configuration["ADD_OPENTRACING"]))
                services.AddOpenTracing ();

            // Adds the Jaeger Tracer.
            services.AddSingleton<ITracer> (serviceProvider => {
                string serviceName = serviceProvider.GetRequiredService<IHostingEnvironment> ().ApplicationName;
                var sender = new Jaeger.Senders.UdpSender (Configuration["JAEGER_AGENT_HOST"], 6831, 0);
                var remoteReporter = new RemoteReporter.Builder ().WithSender (sender).Build ();
                var tracer = new Tracer.Builder (serviceName)
                    .WithSampler (new ConstSampler (sample: true))
                    .WithReporter (remoteReporter)
                    .Build ();

                // Allows code that can't use DI to also access the tracer.
                GlobalTracer.Register (tracer);

                return tracer;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseMvc ();
        }
    }
}