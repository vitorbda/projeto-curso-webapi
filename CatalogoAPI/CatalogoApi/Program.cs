using CatalogoApi.ApiEndpoints;
using CatalogoApi.AppServicesExtensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddApiSwagger()
    .AddPersistence()
    .AddAutenticationJwt()
    .Services
    .AddSwagger()
    .AddCors();

var app = builder.Build();

var environment = app.Environment;

app.UseExceptionHandling(environment)
    .UseSwaggerMiddleware()
    .UseAppCors();

app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
