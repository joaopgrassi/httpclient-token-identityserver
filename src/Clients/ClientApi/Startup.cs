using System;
using ClientApi.ApiClient;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientApi
{
    public class Startup
    {
        public Startup()
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds an instance of the class that contains our credentials
            services.AddSingleton(new ClientCredentialsTokenRequest
            {
                Address = "http://httpclient-idsrv/connect/token",
                ClientId = "client-app",
                ClientSecret = "secret",
                Scope = "read:entity"
            });

            // The DelegatingHandler has to be registered as a Transient Service
            services.AddTransient<ProtectedApiBearerTokenHandler>();

            // Register our ProtectedApi client with a DelegatingHandler that knows how to obtain an access_token
            services.AddHttpClient<IProtectedApiClient, ProtectedApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://protected-api");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();

            // Registers the IdentityServer client
            services.AddHttpClient<IIdentityServerClient, IdentityServerClient>(client =>
            {
                client.BaseAddress = new Uri("http://httpclient-idsrv");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
