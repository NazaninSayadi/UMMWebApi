using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMMDomain;

namespace UMMBusinessLogic.UnitTest.FackeDb
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
       : base(options)
        {
        }
        public DbSet<BaseUnit> BaseUnits => Set<BaseUnit>();
        public DbSet<CoefficientUnit> CoefficientUnits => Set<CoefficientUnit>();
        public DbSet<FormulatedUnit> FormulatedUnits => Set<FormulatedUnit>();

    }
}
