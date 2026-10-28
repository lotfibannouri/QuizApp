using ConceptionQuiz_Api.Repository;

namespace ConceptionQuiz_Api.utility
{
    /// <summary>
    /// Supprime chaque jour les sessions de surveillance plus anciennes que
    /// Proctoring:RetentionDays : sans elle, les captures s'accumulent sur le disque
    /// indéfiniment, alors que ce sont des photos de visages à durée de conservation limitée.
    /// </summary>
    public class ProctoringRetentionService : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromDays(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProctoringRetentionService> _logger;

        public ProctoringRetentionService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ProctoringRetentionService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Une valeur absente ou négative désactive la purge automatique.
            var retentionDays = _configuration.GetValue("Proctoring:RetentionDays", -1);
            if (retentionDays < 0)
            {
                _logger.LogInformation("Purge automatique des captures désactivée (Proctoring:RetentionDays absent).");
                return;
            }

            // Un premier passage au démarrage, puis une fois par jour.
            using var timer = new PeriodicTimer(CheckInterval);
            do
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IProctoringRepository>();

                    var purged = await repository.PurgeSessionsOlderThan(DateTime.Now.AddDays(-retentionDays));
                    if (purged > 0)
                        _logger.LogInformation("{Count} session(s) de surveillance purgée(s) (plus de {Days} jours).", purged, retentionDays);
                }
                catch (Exception ex)
                {
                    // Un échec (base indisponible, fichier verrouillé) ne doit pas arrêter l'API :
                    // le passage suivant réessaiera.
                    _logger.LogError(ex, "Échec de la purge automatique des captures de surveillance.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
