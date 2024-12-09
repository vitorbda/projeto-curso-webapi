
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("filters", (string chaveSecreta) => {
    return "Usando filtros em Minimal Apis";
}).AddEndpointFilter(async (context, next) =>
{
    if (!context.HttpContext.Request.QueryString.Value.Contains("numsey"))
    {
        return Results.BadRequest();
    }

    return await next(context);
});


app.MapGet("/curto-circuito", () => "Nunca será executado...").AddEndpointFilter<ShortCircuit>();


app.MapPost("/upload", async (IFormFile arquivo) =>
{
    await arquivo.CopyToAsync(File.OpenWrite($"{DateTime.Now.Ticks}.txt"));
}).WithMetadata(new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute());

app.MapPost("/uploadArquivo", async (IFormFile arquivo) =>
{
    string arquivoTemp = CriaCaminhoArquivoTemp();
    using var stream = File.OpenWrite(arquivoTemp);
    await arquivo.CopyToAsync(stream);

    return Results.Ok("Arquivo enviado com sucesso");
});

app.MapPost("/uploadArquivos", async (IFormFileCollection arquivos) =>
{
    foreach(var arquivo in arquivos)
    {
        string arquivoTemp = CriaCaminhoArquivoTemp();
        using var stream = File.OpenWrite(arquivoTemp);
        await arquivo.CopyToAsync(stream);
    }

    return Results.Ok("Arquivos enviados com sucesso");
});

app.MapGet("/procurar", (string[] nomes) =>
{
    var result = $"Nomes: {nomes[0]}, {nomes[1]}, {nomes[2]}";

    return Results.Ok(result);
});

app.MapGet("/buscar", ([AsParameters] Livro info) =>
{
    return $"Livro: {info.Titulo}, {info.Autor}, {info.Ano}";
});


static string CriaCaminhoArquivoTemp()
{
    var nomeArquivo = $"{DateTime.Now.Ticks}.tmp";

    var directoryPath = Path.Combine("temp", "uploads");

    if (!Directory.Exists(directoryPath))
        Directory.CreateDirectory(directoryPath);

    return Path.Combine(directoryPath, nomeArquivo);
}

app.UseHttpsRedirection();
app.Run();

public class ShortCircuit : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return new ValueTask<object?>(Results.Json(new { Curto = "Circuito" }));
    }
}

public class Livro
{
    public string? Autor { get; set; }
    public string? Titulo { get; set; }
    public int Ano { get; set; }
}
