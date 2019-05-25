using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Imperium_Incursions_Waitlist.Data;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.Extensions.DependencyInjection;
using Imperium_Incursions_Waitlist.Services;

public class CorporationService : IHostedService
{
    private Timer _timer;
    private WaitlistDataContext _Db;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CorporationService> _logger;  

    public CorporationService(ILogger<CorporationService> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Create a new scope to retrieve scoped services
        var scope = _serviceProvider.CreateScope();
        
        // Get the DbContext instance
        _Db = scope.ServiceProvider.GetRequiredService<WaitlistDataContext>();

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
                  TimeSpan.FromHours(1));
        
    }

    private void DoWork(object state)
    {
        _logger.LogInformation("Background Service Started: updating corporations.");
        List<Corporation> corporations = _Db.Corporation.ToList();
        foreach(Corporation corporation in corporations)
        {
            var result = EsiWrapper.GetCorporation(corporation.Id);
            corporation.Name = result.Result.Name;
            corporation.AllianceId = result.Result.AllianceId;
        }

        _Db.SaveChanges();
        _logger.LogInformation("Background Service Completed: corporations updated.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}