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
    public class FormulatedUnitRepository : IFormulatedUnitRepository
    {
        private readonly Context _context;

        public FormulatedUnitRepository(Context context)
        {
            _context = context;
        }

        public async Task Add(FormulatedUnit formulatedUnit)
        {
            _context.FormulatedUnits.Add(formulatedUnit);
            _context.SaveChanges();
        }

        public async Task<FormulatedUnit> Get(string symbolName)
        {
            var formulatedUnit = _context.FormulatedUnits
                            .Where(f => f.Symbol == symbolName)
                            .SingleOrDefault();

            return formulatedUnit;
        }
    }
}
