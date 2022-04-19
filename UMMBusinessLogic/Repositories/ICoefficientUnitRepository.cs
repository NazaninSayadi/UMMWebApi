using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMMDomain;

namespace UMMBusinessLogic.Repositories
{
    public interface ICoefficientUnitRepository
    {
        Task Add(CoefficientUnit coefficientUnit);
        Task<CoefficientUnit> Get(string symbolName = "", string metricName = "");
    }
}
