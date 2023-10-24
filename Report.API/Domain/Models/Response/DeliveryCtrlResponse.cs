namespace Report.API.Domain.Models.Response
{
    public class DeliveryCtrlResponse
    {
        public class DeliveryCtrl
        {
            public string compCode { get; set; }
            public string brnCode { get; set; }

            public string docNo { get; set; }
            public string docDate { get; set; }
            public string receiveNo { get; set; }
            public string realDate { get; set; }
            public string whName { get; set; }
            public string licensePlate { get; set; }
            public string carNo { get; set; }
            public string empName { get; set; }


            public string ctrlCorrect { get; set; }
            public string ctrlCorrectReasonDesc { get; set; }
            public string ctrlCorrectOther { get; set; }
            public string ctrlFull { get; set; }
            public int ctrlFullMn { get; set; }
            public int ctrlFullLt { get; set; }
            public string ctrlFullContact { get; set; }
            public string ctrlOnTime { get; set; }
            public int ctrlOnTimeLate { get; set; }
            public string ctrlDoc { get; set; }
            public string ctrlDocDesc { get; set; }
            public string ctrlApi { get; set; }
            public string ctrlApiDesc { get; set; }
            public string ctrlEthanol { get; set; }
            public string ctrlSeal { get; set; }
            public string remark { get; set; }


        }
    }


}
