using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace APICatalogo.Controller
{
    [Route("api/teste")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TesteV1Controller : ControllerBase
    {

        [HttpGet]
        public string GetVersion()
        {
            return "TesteV1 - GET - Api Versão 1.0";
        }
    }
}
