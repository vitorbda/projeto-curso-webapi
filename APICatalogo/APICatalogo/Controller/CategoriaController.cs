﻿using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using System.Text;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace APICatalogo.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    //[EnableCors("OrigensComAcessoPermitido")]
    [EnableRateLimiting("fixedwindow")]
    [Produces("application/json")]
    //[ApiExplorerSettings(IgnoreApi = true)]
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
        [Authorize]
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

        /// <summary>
        /// Obtem uma lista de objeto Categoria
        /// </summary>
        /// <returns>Uma lista de objeto Categoria</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Obtem uma categoria pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objetos Categoria</returns>
        //[DisableCors]
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///     POST api/categorias {
        ///         "id": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "teste.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDto">objeto Categoria</param>
        /// <returns>O objeto Categoria incluído</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
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
        [Authorize("AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
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

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategoriasPagination(IPagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.Count,
                categorias.PageSize,
                categorias.PageCount,
                categorias.TotalItemCount,
                categorias.HasNextPage,
                categorias.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(_mapper.Map<IEnumerable<CategoriaDTO>>(categorias));
        }
    }
}
