using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using MaxStation.Entities.Models;
using System.Linq;
using MasterData.API.Helpers;
using MasterData.API.Models;
using MasterData.API.Models.Other;

namespace MasterData.API.Repositories.Other
{
    public interface IDocumentTypeRepositories
    {
        List<MasDocumentType> GetDocumentTypeByDocType(RequestData req);
    }

    public class DocumentTypeRepositories : SqlDataAccessHelper, IDocumentTypeRepositories
    {
        readonly string connectionString;
        readonly AppSettings appSettings;
        public DocumentTypeRepositories(IOptions<AppSettings> AppSettings, PTMaxstationContext context) : base(AppSettings, context) {
            this.appSettings = AppSettings.Value;
            connectionString = this.appSettings.ConnectionString;
        }

        public List<MasDocumentType> GetDocumentTypeByDocType(RequestData req)
        {
            List<MasDocumentType> resp = new List<MasDocumentType>();
            resp = this.context.MasDocumentTypes.Where(x => x.DocTypeDesc == req.DocType && x.DocTypeStatus == "Active").ToList();
            return resp;
        }
    }
}
