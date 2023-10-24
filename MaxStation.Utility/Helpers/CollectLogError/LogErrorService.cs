using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MaxStation.Entities.Models;

namespace MaxStation.Utility.Helpers.CollectLogError
{
    public class LogErrorService : ILogErrorService
    {
        public Task<LogError> ConvertLogError(string logStatus, string compCode, string brnCode, string locCode, string jsonData, string messageLog, DateTime? createdDate, string createdBy, string path, string method, string host, string stackTrace)
        {
            try
            {
                LogError result = new LogError();
                var logError = new LogError
                {
                    LogStatus = logStatus,
                    CompCode = compCode,
                    BrnCode = brnCode,
                    LocCode = locCode,
                    JsonData = jsonData,
                    Message = messageLog,
                    CreatedDate = createdDate,
                    CreatedBy = createdBy,
                    Path = path,
                    Method = method,
                    Host = host,
                    StackTrace = stackTrace
                };
                result = logError;
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when convert log error: {ex.Message}");
            }
        }
    }
}
