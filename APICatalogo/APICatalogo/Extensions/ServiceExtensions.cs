using APICatalogo.Filters;
using APICatalogo.Services;

namespace APICatalogo.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IMeuServico, MeuServico>();

            services.AddScoped<ApiLoggingFilter>();

            return services;
        }
    }
}
