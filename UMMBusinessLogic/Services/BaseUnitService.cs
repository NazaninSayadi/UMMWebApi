using UMMBusinessLogic.Exceptions;
using UMMBusinessLogic.Repositories;
using UMMDomain;

namespace UMMBusinessLogic.Services
{
    public class BaseUnitService : IBaseUnitService
    {
        private readonly IBaseUnitRepository _baseUnitRepository;

        public BaseUnitService(IBaseUnitRepository baseUnitRepository)
        {
            _baseUnitRepository = baseUnitRepository;
        }

        public void CreateBaseUnit(string unitName, string metricName, string symbol)
        {
            var baseUnit = _baseUnitRepository.Get(symbol, metricName).Result;
            if (baseUnit != null)
            {
                if (baseUnit.MetricName == metricName)
                    throw new DuplicateMetricException($"This Metric has another BaseUnit: {baseUnit.Symbol}");
                else
                    throw new DuplicateSymbolException($"The base unit {baseUnit.Symbol} is exist");
            }


            baseUnit = new()
            {
                UnitName = unitName,
                MetricName = metricName,
                Symbol = symbol
            };
            _baseUnitRepository.Add(baseUnit);

        }
        public async Task<BaseUnit> GetBaseUnit(string unitName, string metricName)
        {
            return await _baseUnitRepository.Get(unitName, metricName);
        }

    }
}
