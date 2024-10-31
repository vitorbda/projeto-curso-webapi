using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace APICatalogo.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;
        private readonly Guid _id = Guid.NewGuid();
        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger; 
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var obj = RetornaObjeto(context.ActionArguments);
            _logger.LogInformation($"### Executando -> OnActionExecuting [{_id}]");
            _logger.LogInformation("###############");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
            _logger.LogInformation(obj);
            _logger.LogInformation("#############################");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"### Executando -> OnActionExecuted [{_id}]");
            _logger.LogInformation("###############");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
            _logger.LogInformation("#############################");
        }

        private string RetornaObjeto(IDictionary<string, object> campos)
        {
            var umElemento = campos.Count == 1;
            var nome = $"{campos.Keys.FirstOrDefault()}";
            var stringInicio = "";

            if (umElemento)
                stringInicio = "[{" + $"\"{nome}\": [" + "{";
            else
                stringInicio = "";

            var resultado = new StringBuilder(stringInicio);

            foreach (var item in campos)
            {
                var concatValues = new StringBuilder();
                var valor = item.Value;

                Type tipo = valor?.GetType();
                if (tipo is not null && !tipo.IsPrimitive && tipo != typeof(string) && !tipo.IsValueType && !tipo.IsPrimitive)
                {
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(tipo))
                    {
                        var conv = (System.Collections.IList)valor;

                        for (var i = 0; i < conv.Count; i++)
                        {
                            var aux = i == 0 ? "" : "{";
                            var aux2 = i + 1 == conv.Count ? "" : ",";

                            concatValues.Append( aux + RetornaObjeto(ParaDicionario(conv[i])) + aux2 );
                        }
                    }                                                   
                    else
                    {
                        concatValues.Append(RetornaObjeto(ParaDicionario(valor)));
                    }                        
                }
                else
                {
                    concatValues.Append($" \"{item.Key}\": \"{item.Value}\"");
                }
                    
                resultado.Append(concatValues + ",");
            }

            resultado.Length--;
            if (umElemento)
                resultado.Append("]}]");
            else
                resultado.Append("}");

            return resultado.ToString();
        }


        public static IDictionary<string, object> ParaDicionario<T>(T obj)
        {
            IDictionary<string, object> dicionario = new Dictionary<string, object>();

            Type tipo = typeof(T);
            PropertyInfo[] propriedades = obj.GetType().GetProperties();
            foreach (var propriedade in propriedades)
            {
                var valor = propriedade.GetValue(obj);
                dicionario.Add(propriedade.Name, valor);
            }

            return dicionario;
        }
        
    }
}
