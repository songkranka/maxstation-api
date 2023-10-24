using System;
using System.Threading.Tasks;
using MaxStation.Entities.Models;

namespace MaxStation.Utility.Helpers.CollectLogError
{
    public interface ILogErrorService
    {
        Task<LogError> ConvertLogError(string logStatus, string compCode, string brnCode, string locCode, string jsonData, string messageLog, DateTime? createdDate, string createdBy, string path, string method, string host, string stackTrace);
    }
}
