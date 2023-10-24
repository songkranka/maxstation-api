using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class DeliveryCtrlRepository : SqlDataAccessHelper, IDeliveryCtrlRepository
    {
        private readonly IMapper _mapper;

        public DeliveryCtrlRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }


        public async Task<List<DeliveryCtrlResponse.DeliveryCtrl>> GetDocument(DeliveryCtrlRequest request)
        {
            List<DeliveryCtrlResponse.DeliveryCtrl> result = new List<DeliveryCtrlResponse.DeliveryCtrl>();

            var query = this.context.InvDeliveryCtrlHds.Where(x=>x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.DocNo== request.DocNo).AsQueryable();

            result = await query.Select(x => new DeliveryCtrlResponse.DeliveryCtrl
                            {
                                compCode = x.CompCode,
                                brnCode = x.BrnCode,
                                docNo = x.DocNo,
                                docDate = x.DocDate.Value.ToString("yyyy-MM-dd"),
                                receiveNo = x.ReceiveNo,
                                realDate = x.RealDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                whName = x.WhName,
                                carNo= x.CarNo,
                                licensePlate = x.LicensePlate,
                                empName = x.EmpName,
                                ctrlCorrect = x.CtrlCorrect,
                                ctrlCorrectOther= x.CtrlCorrectOther,
                                ctrlCorrectReasonDesc= x.CtrlCorrectReasonDesc,                                
                                ctrlOnTime = x.CtrlOntime,
                                ctrlOnTimeLate = x.CtrlOntimeLate??0,
                                ctrlDoc = x.CtrlDoc,
                                ctrlDocDesc = x.CtrlDocDesc,
                                ctrlSeal= x.CtrlSeal,
                                remark= x.Remark,
            }).ToListAsync();

            return  result;
        }
    }
}
