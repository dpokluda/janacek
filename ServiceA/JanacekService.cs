using System;
using System.Collections.Generic;
using JanacekClient;
using Microsoft.AspNetCore.Builder;

namespace ServiceA
{
    public static class JanacekService
    {
        public static Dictionary<string, Janacek> registeredServices = new Dictionary<string, Janacek>();
        public static JanacekConsumer Consumer;
        public static JanacekProducer Producer;

        public static JanacekConsumer UseJanacek(
            this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.UseMvc(
                routes => routes.MapRoute("janacek", "janacek/act/"));

            Consumer = new JanacekConsumer(registeredServices);
            Producer = new JanacekProducer();

            return Consumer;
        }
    }
}
