using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;

        public DocumentTypeService(
            IDocumentTypeRepository documentTypeRepository)
        {
            _documentTypeRepository = documentTypeRepository;
        }

        public List<MasDocumentType> GetDocumentTypeByDocType(DocumentTypeRequest req)
        {
            List<MasDocumentType> resp = new List<MasDocumentType>();
            resp = _documentTypeRepository.GetDocumentTypeByDocType(req);
            return resp;
        }
    }
}
