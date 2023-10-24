using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class DocumentTypeRepository : SqlDataAccessHelper, IDocumentTypeRepository
    {
        public DocumentTypeRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasDocumentType> GetDocumentTypeByDocType(DocumentTypeRequest req)
        {
            List<MasDocumentType> resp = new List<MasDocumentType>();
            resp = this.context.MasDocumentTypes.Where(x => x.DocTypeDesc == req.DocType && x.DocTypeStatus == "Active").ToList();
            return resp;
        }
    }
}
