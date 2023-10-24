using Abp.Linq.Expressions;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Position;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class PositionService : IPositionService
    {
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;
        public PositionService(PTMaxstationContext pContext, IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
        }
        public async Task<ModelGetPositionListResult> GetPositionList(ModelGetPositionListParam param)
        {
            if(param == null)
            {
                return null;
            }
            var result = new ModelGetPositionListResult();
            var qryPosition = _context.MasPositions
                .OrderBy(x => x.PositionCode.Length)
                .ThenBy(x=> x.PositionCode)
                .AsNoTracking();

            string strKeyword = (param?.Keyword ?? string.Empty).Trim();
            if(!0.Equals(strKeyword.Length))
            {
                var esPos = PredicateBuilder.New<MasPosition>(x=>x.PositionCode.Contains(strKeyword));
               // esPos = esPos.Or(x => strKeyword.Contains(x.PositionCode));
                esPos = esPos.Or(x => !string.IsNullOrEmpty(x.PositionName) && x.PositionName.Contains(strKeyword));
                //esPos = esPos.Or(x => strKeyword.Contains(x.PositionName));
                esPos = esPos.Or(x => !string.IsNullOrEmpty(x.PositionDesc) && x.PositionDesc.Contains(strKeyword));
                //esPos = esPos.Or(x => strKeyword.Contains(x.PositionDesc));
                qryPosition = qryPosition.Where(esPos);
            }
            result.TotalItem = await qryPosition.CountAsync();
            if (param.ItemsPerPage > 0 && param.Page > 0)
            {
                qryPosition = qryPosition
                    .Skip((param.Page - 1) * param.ItemsPerPage)
                    .Take(param.ItemsPerPage);
            }
            result.ArrPosition = await qryPosition.ToArrayAsync();            
            return result;
        }

        public async Task<ModelPosition> GetPosition(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            MasPosition pos = null;
            AutPositionRole[] arrRole = null;

            var qryPos = _context.MasPositions.Where(x => x.Guid.ToString() == pStrGuid).AsNoTracking();
            pos = await qryPos.FirstOrDefaultAsync();
            if(pos != null)
            {
                string strPosCode = string.Empty;
                strPosCode = DefaultService.GetString(pos.PositionCode);
                if (!0.Equals(strPosCode.Length))
                {
                    var qryPosRole = _context.AutPositionRoles.Where(x => x.PositionCode == strPosCode).AsNoTracking();
                    arrRole = await qryPosRole.ToArrayAsync();
                }
            }
            var result = new ModelPosition();
            result.Position = pos;
            result.ArrPositionRole = arrRole;
            return result;
        }

        public async Task<ModelPosition> InsertPosition(ModelPosition pInput)
        {
            if(pInput == null)
            {
                return null;
            }
            string strPosCode = "0";           
            int intLastPosCode = 0;
            var qryPos = _context.MasPositions
                .OrderByDescending(x => x.PositionCode.Length)
                .ThenByDescending(x => x.PositionCode)
                .AsNoTracking();
            var lastPos = await qryPos.FirstOrDefaultAsync();
            if(lastPos != null)
            {
                intLastPosCode = Convert.ToInt32(lastPos.PositionCode ?? strPosCode);
                
            }
            bool isDuplicatePos = false;
            bool isDuplicateRole = false;
            do
            {
                strPosCode = (++intLastPosCode).ToString();
                isDuplicatePos = await _context.MasPositions.AnyAsync(x => x.PositionCode == strPosCode);
                isDuplicateRole = await _context.AutPositionRoles.AnyAsync(x => x.PositionCode == strPosCode);
            } while (isDuplicatePos || isDuplicateRole);
            if (pInput.Position != null)
            {
                var pos = pInput.Position;
                pos.PositionCode = strPosCode;
                pos.Guid = Guid.NewGuid();
                pos.CreatedDate = DateTime.Now;
                pos.UpdatedDate = DateTime.Now;
                await _context.MasPositions.AddAsync(pos);
            }
            if(pInput.ArrPositionRole != null && pInput.ArrPositionRole.Length > 0)
            {
                pInput.ArrPositionRole = pInput.ArrPositionRole.Where(x => x != null).ToArray();
                foreach (var role in pInput.ArrPositionRole)
                {                    
                    role.PositionCode = strPosCode;
                    role.CreatedDate = DateTime.Now;
                    role.UpdatedDate = DateTime.Now;
                }
                await _context.AutPositionRoles.AddRangeAsync(pInput.ArrPositionRole);
            }        
            await _unitOfWork.CompleteAsync();
            return pInput;
        }

        public async Task<SaveUnlock> InsertUnlock(SaveUnlock pInput)
        {
            if (pInput == null)
            {
                return null;
            }
            var sysBranchConfigRoles = new List<SysBranchConfigRole>();

            foreach (var unlockPosition in pInput._UnlockPosition)
            {
                var sysBranchConfigRole = new SysBranchConfigRole()
                {
                    PositionCode = pInput.PositionCode,
                    ItemNo = unlockPosition.ItemNo,
                    ConfigId = unlockPosition.ConfigId,
                    IsView = unlockPosition.IsView,
                    CreatedDate = DateTime.Now
                };
                sysBranchConfigRoles.Add(sysBranchConfigRole);
            }

            await _context.SysBranchConfigRoles.AddRangeAsync(sysBranchConfigRoles);

            await _unitOfWork.CompleteAsync();
            return pInput;
        }

        public async Task<ModelPosition> UpdatePosition(ModelPosition pInput)
        {
            if (pInput == null)
            {
                return null;
            }            
            if(pInput.Position != null)
            {
                pInput.Position.UpdatedDate = DateTime.Now;                
                var entPos = _context.Entry(pInput.Position);
                entPos.State = EntityState.Modified;
                entPos.Property(x => x.CreatedBy).IsModified = false;
                entPos.Property(x => x.CreatedDate).IsModified = false;
                entPos.Property(x => x.Guid).IsModified = false;
            }
            if(pInput.ArrPositionRole != null && pInput.ArrPositionRole.Length > 0)
            {
                pInput.ArrPositionRole = pInput.ArrPositionRole.Where(x => x != null).ToArray();
                var arrPosCode = pInput.ArrPositionRole
                    .Where(x=> !string.IsNullOrEmpty(x.PositionCode))
                    .Select(x => x.PositionCode.Trim())
                    .Where(x=> x.Length > 0)
                    .Distinct().ToArray();
                if(arrPosCode != null && arrPosCode.Length > 0)
                {
                    var qryRole = _context.AutPositionRoles
                        .Where(x =>  arrPosCode.Contains(x.PositionCode))
                        .AsNoTracking();
                    _context.AutPositionRoles.RemoveRange(qryRole);
                }
                foreach (var role in pInput.ArrPositionRole)
                {
                    role.UpdatedDate = DateTime.Now;                    
                }
                await _context.AutPositionRoles.AddRangeAsync(pInput.ArrPositionRole);
            }
            await _unitOfWork.CompleteAsync();
            return pInput;
        }

        public async Task<SaveUnlock> UpdateUnlock(SaveUnlock pInput)
        {
            var masBranchConfigRoles = _context.SysBranchConfigRoles.Where(x => x.PositionCode == pInput.PositionCode).AsNoTracking(); ;
            
            if(masBranchConfigRoles != null)
            {
                _context.SysBranchConfigRoles.RemoveRange(masBranchConfigRoles);

                var sysBranchConfigRoles = new List<SysBranchConfigRole>();
                foreach (var unlockPosition in pInput._UnlockPosition)
                {
                    var sysBranchConfigRole = new SysBranchConfigRole()
                    {
                        PositionCode = pInput.PositionCode,
                        ItemNo = unlockPosition.ItemNo,
                        ConfigId = unlockPosition.ConfigId,
                        IsView = unlockPosition.IsView,
                        UpdatedDate = DateTime.Now
                    };
                    sysBranchConfigRoles.Add(sysBranchConfigRole);
                }
                await _context.SysBranchConfigRoles.AddRangeAsync(sysBranchConfigRoles);
            }

            await _unitOfWork.CompleteAsync();
            return pInput;
        }

        public async Task<SysMenu[]> GetSysMenuList()
        {
            IQueryable<SysMenu> qryMenu = null;
            qryMenu = _context.SysMenus.Where(
                x => !(string.IsNullOrEmpty(x.Child))
            ).AsNoTracking();

            SysMenu[] result = null;
            result = await qryMenu.ToArrayAsync();

            return result;
        }

        public async Task<bool> ChangeStatus(MasPosition param)
        {
            if(param == null)
            {
                return false;
            }
            var entPos = _context.Attach(param);
            param.UpdatedDate = DateTime.Now;
            entPos.Property(x => x.PositionStatus).IsModified = true;
            var qryRole = _context.AutPositionRoles.Where(x => x.PositionCode == param.PositionCode);
            var arrRole = await qryRole.ToArrayAsync();
            foreach (var role in arrRole)
            {
                role.UpdatedBy = param.UpdatedBy;
                role.UpdatedDate = DateTime.Now;
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<List<BranchConfig>> GetBranchConfig(string pStrPosCode)
        {
            var response = new List<BranchConfig>();
            var masPosition = await _context.MasPositions.FirstOrDefaultAsync(x => x.Guid.ToString() == pStrPosCode);
            
            if(masPosition != null)
            {
                var branchConfigs = (from d in _context.MasBranchConfigDescs
                                  join r in _context.SysBranchConfigRoles on new { d.ConfigId } equals new { r.ConfigId }
                                  where (r.PositionCode == masPosition.PositionCode)
                                  select new BranchConfig()
                                  {
                                      ItemNo = d.ItemNo,
                                      ConfigId = d.ConfigId,
                                      ConfigName = d.ConfigName,
                                      IsView = r.IsView,
                                  }).ToListAsync();

                if (branchConfigs.Result.Count > 0 )
                {
                    response = branchConfigs.Result;
                }
                else
                {
                    var masBranchConfigDescs = await _context.MasBranchConfigDescs.ToListAsync();

                    if (masBranchConfigDescs != null)
                    {
                        foreach (var masBranchConfigDesc in masBranchConfigDescs)
                        {
                            var branchConfig = new BranchConfig()
                            {
                                ItemNo = masBranchConfigDesc.ItemNo,
                                ConfigId = masBranchConfigDesc.ConfigId,
                                ConfigName = masBranchConfigDesc.ConfigName,
                                IsView = "N"
                            };
                            response.Add(branchConfig);
                        }
                    }

                }
            }
            
            return response;
        }

        public async Task<List<BranchConfig>> GetBranchConfigDesc()
        {
            var response = new List<BranchConfig>();
            var masBranchConfigDescs = await _context.MasBranchConfigDescs.ToListAsync();

            if(masBranchConfigDescs != null)
            {
                foreach(var masBranchConfigDesc in masBranchConfigDescs)
                {
                    var branchConfig = new BranchConfig()
                    {
                        ItemNo = masBranchConfigDesc.ItemNo,
                        ConfigId = masBranchConfigDesc.ConfigId,
                        ConfigName = masBranchConfigDesc.ConfigName,
                        IsView = "N"
                    };
                    response.Add(branchConfig);
                }
            }


            return response;
        }
    }
}
