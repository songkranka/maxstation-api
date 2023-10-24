using Finance.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.API.Domain.Models.Request
{
    public class SaveExpenseRequest
    {
        public SaveFinExpenseHd FinExpenseHd { get; set; }
        public List<ExpenseTable> ExpenseTables { get; set; }
        public List<ExpenseEssTable> ExpenseEssTables { get; set; }
    }
    public class SaveFinExpenseHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime DocDate { get; set; }
        public string WorkType { get; set; }
        public string WorkStart { get; set; }
        public string WorkFinish { get; set; }
        public string Remark { get; set; }
        public string Post { get; set; }
        public int RunNumber { get; set; }
        public string DocPattern { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
