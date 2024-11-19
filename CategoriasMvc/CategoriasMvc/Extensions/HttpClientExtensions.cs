using System.Net.Http.Headers;

namespace CategoriasMvc.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddHttpClient(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddHttpClient("CategoriasApi", c =>
            {
                c.BaseAddress = new Uri(config["ServiceUri:CategoriasApi"]);
            });

            services.AddHttpClient("AutenticaApi", c =>
            {
                c.BaseAddress = new Uri(config["ServiceUri:AutenticaApi"]);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient("ProdutosApi", c =>
            {
                c.BaseAddress = new Uri(config["ServiceUri:ProdutosApi"]);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }
    }
}
