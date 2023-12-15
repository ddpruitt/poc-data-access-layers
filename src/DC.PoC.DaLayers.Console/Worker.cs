namespace DC.PoC.DaLayers.Console
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHost _host;

        public Worker(
            ILogger<Worker> logger,
            IHost host)
        {
            _logger = logger;
            _host = host;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var x = new {Index = i, Now = DateTime.UtcNow, Value = $"Test{i:00}"};

                        _logger.LogInformation("Created: {@x}",x);
                    }
                    
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload terminated unexpectedly");
            }
            finally
            {
                await _host.StopAsync(stoppingToken);
            }
        }
    }
}