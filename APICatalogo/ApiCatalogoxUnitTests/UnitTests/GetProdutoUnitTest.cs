using APICatalogo.Controller;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class GetProdutoUnitTest : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;

        public GetProdutoUnitTest(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task GetProdutoById_Return_OkResult()
        {
            //Arrange
            var prodId = 2;

            //Act
            var data = await _controller.Get(prodId);

            //Assert (xUnit)
            //var okResult = Assert.IsType<OkObjectResult>(data.Result);
            //Assert.Equal(200, okResult.StatusCode);

            //Assert (fluentassertions)
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }
    }
}
