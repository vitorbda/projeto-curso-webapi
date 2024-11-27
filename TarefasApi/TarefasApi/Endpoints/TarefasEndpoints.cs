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

            return GenericGetReturn(tarefas);
        });

        app.MapGet("/tarefas/{id}", async (GetConnection connectionGetter, int id) =>
        {
            using var con = await connectionGetter();
            var tarefa = con.Get<Tarefa>(id);
            return GenericGetReturn(tarefa);
        });

        app.MapPost("/tarefas", async (GetConnection connectionGetter, Tarefa tarefa) =>
        {
            using var con = await connectionGetter();
            return Results.Created($"/tarefas/{con.Insert<Tarefa>(tarefa)}", tarefa);
        });

        app.MapPut("/tarefas", async (GetConnection connectionGetter, Tarefa tarefa) =>
        {
            using var con = await connectionGetter();
            _ = con.Update(tarefa);
            return Results.Ok();
        });

        app.MapDelete("/tarefas/{id}", async (GetConnection connectionGetter, int id) =>
        {
            using var con = await connectionGetter();
            var tarefa = con.Get<Tarefa>(id);

            if (tarefa is null) return Results.NotFound();

            con.Delete(tarefa);

            return Results.Ok(tarefa);
        });
    }

    private static IResult GenericGetReturn(object obj) => obj is null ? Results.NotFound() : Results.Ok(obj);

}
