using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentGateway.API.Mappings;
using PaymentGateway.API.Models;
using PaymentGateway.API.Validation;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Commands;
using PaymentGateway.Domain.Persistence;
using PaymentGateway.Domain.Services;
using Prometheus;

namespace PaymentGateway.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options => options.ModelValidatorProviders.Clear())
                .AddFluentValidation()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services
                .AddMediatR(typeof(ProcessPaymentCommand))
                .AddTransient<IPaymentService, PaymentService>()
                .AddTransient<IBankService, MockBankService>()
                .AddSingleton<IPaymentRepository, InMemoryPaymentRepository>()
                .AddTransient<IMapper, Mapper>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>()
                .AddTransient<IValidator<PaymentRequest>, PaymentRequestValidator>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "PaymentGateway.API", Version = "v1"});
            });
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway.API v1"));
            }

            app.UseHttpMetrics();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseHealthChecks("/ready");
        }
    }
}
