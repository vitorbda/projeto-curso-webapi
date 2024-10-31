
namespace APICatalogo.Logging
{
    public class CustomerLogger : ILogger
    {
        private readonly string loggerName;
        private readonly CustomLoggerProviderConfiguration loggerConfiguration;
        private readonly string caminho;

        public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
        {
            loggerName = name;
            loggerConfiguration = config;

            string caminho = Path.Combine(Directory.GetCurrentDirectory(), "AppLog.txt");

            this.caminho = caminho;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == loggerConfiguration.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var mensagem = $"{logLevel.ToString()}: {eventId.Id} = {formatter(state, exception)}";

            EscreverTextoNoArquivo(mensagem);
        }

        private void EscreverTextoNoArquivo(string mensagem)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(caminho))
                {
                    streamWriter.WriteLine(mensagem);
                    streamWriter.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
