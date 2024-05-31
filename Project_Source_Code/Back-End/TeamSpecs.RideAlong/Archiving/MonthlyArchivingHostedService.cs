using TeamSpecs.RideAlong.LoggingLibrary;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using TeamSpecs.RideAlong.Model;
using Org.BouncyCastle.Asn1;
using System.Reflection.Metadata.Ecma335;

namespace TeamSpecs.RideAlong.Archiving
{
    public class MonthlyArchivingHostedService : IReoccurringArchivingHostedService
    {
        private ILogService _logger;
        private IArchivingService _aService;
        public MonthlyArchivingHostedService(ILogService logger, IArchivingService aService)
        {
            _logger = logger;
            _aService = aService;
        }
        
        private async Task<bool> ExecuteArchivingAsync(DateTime before, DateTime? notBefore)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _logger.CreateLogAsync("Debug", "Business", "Archiving Hosted Service has begun", null);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            IResponse archivingResponse = _aService.GetArchive(before, notBefore);

            if (archivingResponse.HasError)
            {
                if (archivingResponse.ErrorMessage is not null)
                {
                    Console.WriteLine($"Could not get logs to be archived: {archivingResponse.ErrorMessage}");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _logger.CreateLogAsync("Error", "Business", $"Could not archive logs: {archivingResponse.ErrorMessage}", null);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                else
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _logger.CreateLogAsync("Error", "Business", $"Could not archive logs, Unknown error occurred", null);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ByteArrayContent content = new ByteArrayContent((byte[])archivingResponse.ReturnValue!.First());
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    HttpResponseMessage response = await client.PostAsync("rideAlong.lol:8088/Archiving/storeLogs", content);
                }
            }
            catch
            {

            }

            return true;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            while (!cancellationToken.IsCancellationRequested)
            {
                // Find dates of first of current month and first of month prior to that
                DateTime today = DateTime.Now;
                DateTime nextRunTIme = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1);
                DateTime archiveLogsBefore = nextRunTIme.AddMonths(-1);
                DateTime butNotBefore = archiveLogsBefore.AddMonths(-1);

                var timer = new Stopwatch();

                // Find time until the first of next month and wait until then
                TimeSpan delay = nextRunTIme - DateTime.Now;
                
                // Wait until time to execute
                Task.Delay(delay).Wait();

                // Begin Archiving
                IResponse archivingResponse = new Response();
                var source = new CancellationTokenSource();
                Task<bool> archivingTask = Task.Run(() => ExecuteArchivingAsync(archiveLogsBefore, butNotBefore), source.Token);
                source.CancelAfter(5000);
                try
                {
                    archivingTask.Wait();
                    if (archivingTask.Result)
                    {
                        _logger.CreateLogAsync("Info", "Business", "Logging finished successfully", null);
                    }
                    else
                    {
                        _logger.CreateLogAsync("Error", "Business", "Logging Failed to complete!", null);
                    }
                }
                catch
                {
                    _logger.CreateLogAsync("Error", "Server", "Something went wrong while logging", null);
                }
                return Task.CompletedTask;
            }
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.CreateLogAsync("Debug", "Business", "Archiving Hosted Service Shutting Down", null);
            return Task.CompletedTask;
        }

    }
}
