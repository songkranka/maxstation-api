using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Models
{
    public class RequestWarpadTaskList
    {
        public string User { get; set; }
    }

    public class ResponseWarpadTaskList
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int CurrentCount { get; set; }
        public List<ResponseWarpadTaskData> Data { get; set; }
    }

    public class ResponseWarpadTaskData
    {
        public string Topic { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BranchFrom { get; set; }
        public string Link { get; set; }
        public string Detail { get; set; }
        public string TaskStatus { get; set; }
    }

    public class ModelGetToDoTaskData
    {
        public string AssignID { get; set; }
        public string TaskID { get; set; }
        public string TaskName { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string TaskStatus { get; set; }
        public object GroupId { get; set; }
        public int IsGroup { get; set; }
        public string TaskType { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string TaskTopic { get; set; }
        public ModelGetToDoTaskSecond Second { get; set; }
        public string Link { get; set; }
    }

    public class ModelGetToDoTaskResult
    {
        public int status { get; set; }
        public int resultCode { get; set; }
        public string resultMessage { get; set; }
        public int count { get; set; }
        public List<ModelGetToDoTaskData> resultData { get; set; }
    }

    public class ModelGetToDoTaskSecond
    {
        public int LeftTime { get; set; }
    }

    public class ModelGenerateTokenData
    {
        public string accessToken { get; set; }
        public string accessRefreshToken { get; set; }
    }

    public class ModelGenerateTokenResult
    {
        public int status { get; set; }
        public int resultCode { get; set; }
        public string resultMessage { get; set; }
        public ModelGenerateTokenData resultData { get; set; }
    }
}
