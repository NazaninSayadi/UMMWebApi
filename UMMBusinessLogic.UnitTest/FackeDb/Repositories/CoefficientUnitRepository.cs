using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMMBusinessLogic.Repositories;
using UMMBusinessLogic.UnitTest.FackeDb;
using UMMDomain;

namespace UMMBusinessLogic.UnitTest.FakeDb.Repositories
{
    public class CoefficientUnitRepository : ICoefficientUnitRepository
    {
        private readonly Context _context;

        public CoefficientUnitRepository(Context context)
        {
            _context = context;
        }
        public async Task Add(CoefficientUnit coefficientUnit)
        {
            _context.CoefficientUnits.Add(coefficientUnit);
            _context.SaveChanges();
        }

        public async Task<CoefficientUnit> Get(string symbolName = "", string metricName = "")
        {
            var coefficientUnit = _context.CoefficientUnits
            .Where(c=>c.Symbol == symbolName)
            .SingleOrDefault();

            return coefficientUnit;
        }
    }
}
