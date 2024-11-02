using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Text;

namespace APICatalogo.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriaController(IUnitOfWork uof, IConfiguration configuration, ILogger<CategoriaController> logger)
        {
            _uof = uof;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public ActionResult<string> GetValores()
        {
            _logger.LogInformation("====================== Categoria/GetValores ==============================");

            var valor1 = _configuration["chave1"];
            var valor2 = _configuration.GetValue<string>("chave2");

            var secao1 = _configuration.GetValue<string>("secao1:chave1");
            var secao1_ = _configuration["secao1:chave2"];

            return ConcatenaValores(valor1, valor2, secao1, secao1_);
        }

        [HttpGet("UsandoFromServices/{nome}")]
        public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            var categorias = _uof.CategoriaRepository.Get();
            return Ok(categorias);
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                return Ok(_uof.CategoriaRepository.Get());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }                        
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult Get(int id) 
        {
            try
            {
                var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);

                if (categoria is null)                
                    return NotFound("Categoria não encontrada.");
                
                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            try
            {
                if (categoria is null)                
                    return BadRequest();                

                categoria = _uof.CategoriaRepository.Create(categoria);
                _uof.Commit();

                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoria.Id }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            try
            {
                if (id != categoria.Id)                
                    return BadRequest();

                _uof.CategoriaRepository.Update(categoria);
                _uof.Commit();

                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }

        }

        [HttpDelete]
        public ActionResult<Categoria> Delete(int id) 
        {
            try
            {
                var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);
                if (categoria is null)
                    return NotFound("Categoria não encontrada");

                var categoriaDeletada = _uof.CategoriaRepository.Delete(categoria);
                _uof.Commit();

                return Ok(categoriaDeletada);
            }
            catch 
            {
                throw;
            }            
        }


        private string ConcatenaValores(params string[] valores)
        {
            var valoresConcatenados = new StringBuilder();

            for (int i = 0; i < valores.Length; i++)
                valoresConcatenados.Append($"valor({i + 1}) = {valores[i]} \n");

            return valoresConcatenados.ToString();
        }
    }
}
