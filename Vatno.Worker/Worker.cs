using Newtonsoft.Json;
using Quartz;
using System.Diagnostics;
using Vatno.Worker.Domain.Models.Services;
using IBlobStorageService = Vatno.Worker.BlobStorage.IBlobStorageService;

namespace Vatno.Worker;

public class Worker : IJob
{
    private readonly IVatNoService _vatNoService;
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger _logger;

    public Worker(IVatNoService vatNoService, IBlobStorageService blobStorageService, ILogger<Worker> logger)
    {
        _vatNoService = vatNoService;
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"--------- Start Process Export Log Vatno Maxme: {DateTime.Now:dd-MM-yyyy HH:mm:ss} ---------");
        var fileName = "VatNo";

        var stWatch = new Stopwatch();
        try
        {
            stWatch.Start();
            var result = await _vatNoService.VatExportLog();
            stWatch.Stop();
            _logger.LogInformation($"Query data: [{TimeSpan.FromTicks(stWatch.ElapsedTicks)}]");
            var convertJson = JsonConvert.SerializeObject(result);
            var jsonFile = _blobStorageService.ConvertStringToIFormFile(convertJson, fileName, "application/json; charset=utf-8");

            stWatch.Restart();
            await _blobStorageService.SASUploadAsync($"{fileName}.json", jsonFile);
            stWatch.Stop();
            _logger.LogInformation($"Upload file: [{TimeSpan.FromTicks(stWatch.ElapsedTicks)}]");

            stWatch.Restart();
            await _vatNoService.SaveLogVatNoAsync(fileName, "S", null);
            stWatch.Stop();
            _logger.LogInformation($"Write log: [{TimeSpan.FromTicks(stWatch.ElapsedTicks)}]");
            _logger.LogInformation("--------- End Process Export Completed ---------");
        }
        catch (Exception ex)
        {
            await _vatNoService.SaveLogVatNoAsync(fileName, "E", ex.Message);
            _logger.LogError($"End Process Export VAT Error: {ex}", ex);
        }
    }
}