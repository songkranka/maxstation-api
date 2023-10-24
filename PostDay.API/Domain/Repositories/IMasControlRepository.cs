using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Repositories
{
    public interface IMasControlRepository
    {
        Task UpdateWDateAsync(string compCode, string brnCode, string locCode, string ctrlCode, string ctrlValue);
    }
}
