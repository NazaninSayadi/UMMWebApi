using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMMDomain;

namespace UMMBusinessLogic.Repositories
{
    public interface IBaseUnitRepository
    {
        Task Add(BaseUnit baseUnit);
        Task<BaseUnit> Get(string symbolName = "", string metricName = "");

    }
}
