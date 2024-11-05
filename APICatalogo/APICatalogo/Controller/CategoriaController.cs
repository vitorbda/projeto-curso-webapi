using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly IMapper _mapper;

        public CategoriaController(IUnitOfWork uof, IConfiguration configuration, ILogger<CategoriaController> logger, IMapper mapper)
        {
            _uof = uof;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
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
        public async Task<ActionResult<string>> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }

        [HttpGet("PaginationFilter")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetPaginationFilter([FromQuery] CategoriasFiltroNome categoriasFiltro)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasFiltroAsync(categoriasFiltro);
            if (categorias is null)
                return NotFound();

            return ObterCategoriasPagination(categorias);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters parameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(parameters);

            return ObterCategoriasPagination(categorias);
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            try
            {
                var categorias = await _uof.CategoriaRepository.GetAsync();

                var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

                return Ok(categorias);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }            
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            try
            {
                var categorias = await _uof.CategoriaRepository.GetAsync();

                var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

                return Ok(categoriasDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }                        
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id) 
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetAsync(c => c.Id == id);

                if (categoria is null)                
                    return NotFound("Categoria não encontrada.");

                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {
            try
            {
                if (categoriaDto is null)                
                    return BadRequest();

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                categoriaDto.Id = _uof.CategoriaRepository.Create(categoria).Id;
                await _uof.CommitAsync();

                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoriaDto.Id }, categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            try
            {
                if (id != categoriaDto.Id)                
                    return BadRequest();

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _uof.CategoriaRepository.Update(categoria);
                await _uof.CommitAsync();

                return Ok(categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }

        }

        [HttpDelete]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id) 
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetAsync(c => c.Id == id);
                if (categoria is null)
                    return NotFound("Categoria não encontrada");

                var categoriaDeletada = _uof.CategoriaRepository.Delete(categoria);
                await _uof.CommitAsync();

                var categoriaDeletadaDto = _mapper.Map<CategoriaDTO>(categoriaDeletada);

                return Ok(categoriaDeletadaDto);
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

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategoriasPagination(PagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(_mapper.Map<IEnumerable<CategoriaDTO>>(categorias));
        }
    }
}
