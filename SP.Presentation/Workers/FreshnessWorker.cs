using Microsoft.EntityFrameworkCore;
using SP.Infrastructure.Data;

namespace SP.Presentation.Workers;

public class FreshnessWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<FreshnessWorker> _logger;

    public FreshnessWorker(IServiceProvider services, ILogger<FreshnessWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Le Worker de fraîcheur des spots a démarré !");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Vérification et décrémentation des scores de fraîcheur...");

                // Comme le worker tourne en arrière-plan, il doit créer son propre scope pour parler à la DB
                using (var scope = _services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    //Une seule requête SQL brute ultra rapide
                    // On retire 2 points à tous les spots dont la dernière interaction date d'au moins 1 jour
                    // Et on s'assure que le score ne descend jamais en dessous de 0
                    await context.Database.ExecuteSqlRawAsync(
                        @"UPDATE ""Spots"" 
      SET ""FreshnessScore"" = GREATEST(0::integer, ""FreshnessScore"" - 2) 
      WHERE ""LastVerifiedAt"" < NOW() - INTERVAL '1 day'", 
                        stoppingToken
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la fraîcheur des spots.");
            }

            // Le worker s'endort pendant 24 heures avant de recommencer
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}