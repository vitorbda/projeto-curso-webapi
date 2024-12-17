using AlunosApi.Models;
using AlunosApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlunosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlunosController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Aluno>>> GetAlunos()
        {
            try
            {
                var alunos = await _alunoService.GetAlunos();
                return Ok(alunos);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter alunos");
            }
        }

        [HttpGet("{nome}")]
        public async Task<ActionResult<IAsyncEnumerable<Aluno>>> GetAlunosByName(string nome)
        {
            try
            {
                var alunos = await _alunoService.GetAlunosByNome(nome);

                return alunos is null || !alunos.Any()
                    ? NotFound($"Não existem alunos com o critério {nome}")
                    : Ok(alunos);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter alunos");
            }
        }

        [HttpGet("{id:int}", Name = "GetAluno")]
        public async Task<ActionResult<Aluno>> GetAluno(int id)
        {
            try
            {
                var aluno = await _alunoService.GetAluno(id);

                return aluno is null
                    ? NotFound($"Não existem alunos com o id {id}")
                    : Ok(aluno);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter aluno");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAluno(Aluno aluno)
        {
            try
            {
                await _alunoService.CreateAluno(aluno);
                return CreatedAtRoute(nameof(GetAluno), new { id = aluno.Id }, aluno);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar aluno");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAluno(int id, Aluno aluno)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (aluno.Id != id) return BadRequest("Ids divergentes");

                await _alunoService.UpdateAluno(aluno);
                return Ok(aluno);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar aluno");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAluno(int id)
        {
            try
            {
                var aluno = await _alunoService.GetAluno(id);
                if (aluno is null) return BadRequest("Aluno não encontrado");

                await _alunoService.DeleteAluno(aluno);

                return Ok(aluno);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao deletar aluno");
            }
        }
    }
}
