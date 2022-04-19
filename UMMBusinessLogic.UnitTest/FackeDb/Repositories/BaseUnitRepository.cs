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
    public class BaseUnitRepository : IBaseUnitRepository
    {
        private readonly Context _context;

        public BaseUnitRepository(Context context)
        {
            _context = context;
        }
        public async Task Add(BaseUnit baseUnit)
        {
            _context.BaseUnits.Add(baseUnit);
            _context.SaveChanges();
        }

        public async Task<BaseUnit> Get(string symbolName, string metricName)
        {
            var baseUnit = _context.BaseUnits
               .Where(b => b.Symbol == symbolName || b.MetricName == metricName)
               .SingleOrDefault();

            return baseUnit;
        }
    }
}