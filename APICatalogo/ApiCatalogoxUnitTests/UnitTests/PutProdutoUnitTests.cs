using APICatalogo.Controller;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PutProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;

        public PutProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PutProduto_Return_OkResult()
        {
            //Arrange
            var prodId = 14;

            var updatedProdutoDto = new ProdutoDTO
            {
                Id = prodId,
                Nome = "Produto atualizado",
                Descricao = "Minha descricao",
                ImagemUrl = "imagem1.jpg",
                CategoriaId = 2
            };

            //Act
            var result = await _controller.Put(prodId, updatedProdutoDto) as ActionResult<ProdutoDTO>;

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutProduto_Return_BadRequest()
        {
            //Arrange
            var prodId = 134334;

            var updatedProdutoDto = new ProdutoDTO
            {
                Id = 14,
                Nome = "Produto atualizado",
                Descricao = "Minha descricao",
                ImagemUrl = "imagem1.jpg",
                CategoriaId = 2
            };

            //Act
            var data = await _controller.Put(prodId, updatedProdutoDto) as ActionResult<ProdutoDTO>;

            //Assert
            data.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400);
        }
    }
}
