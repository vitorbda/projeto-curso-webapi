using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
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
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CategoriaController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public ActionResult<string> GetValores()
        {
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
            return _context.Categoria.Include(x => x.Produtos).AsNoTracking().ToList();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            try
            {
                return await _context.Categoria?.AsNoTracking().ToListAsync();
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
                var categoria = _context.Categoria?.AsNoTracking().FirstOrDefault(x => x.Id == id);

                if (categoria is null)
                {
                    return NotFound("Produto não encontrado.");
                }

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
                {
                    return BadRequest();
                }

                _context.Categoria?.Add(categoria);
                _context.SaveChanges();

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
                {
                    return BadRequest();
                }

                _context.Entry(categoria).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
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
