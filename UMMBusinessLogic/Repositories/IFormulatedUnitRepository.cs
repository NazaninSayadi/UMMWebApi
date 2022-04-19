using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMMDomain;

namespace UMMBusinessLogic.Repositories
{
    public interface IFormulatedUnitRepository
    {
        Task Add(FormulatedUnit formulatedUnit);
        Task<FormulatedUnit> Get(string symbolName);
    }
}
