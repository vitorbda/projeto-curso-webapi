using Dapper.Contrib.Extensions;
using TarefasApi.Data;
using static TarefasApi.Data.TarefaContext;
namespace TarefasApi.Endpoints;

public static class TarefasEndpoints
{

    public static void MapTarefasEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => $"Bem-Vindo a API tarefas {DateTime.Now}");

        app.MapGet("/tarefas", async (GetConnection connectionGetter) =>
        {
            using var con = await connectionGetter();
            var tarefas = con.GetAll<Tarefa>().ToList();

            return tarefas is null ? Results.NotFound() : Results.Ok(tarefas);
        });
    }
}
