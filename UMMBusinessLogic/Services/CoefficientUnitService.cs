using UMMBusinessLogic.Exceptions;
using UMMBusinessLogic.Repositories;
using UMMDomain;

namespace UMMBusinessLogic.Services
{
    public class CoefficientUnitService : ICoefficientUnitService
    {
        private readonly ICoefficientUnitRepository _coefficientUnitRepository;

        private readonly IBaseUnitRepository _baseUnitRepository;

        public CoefficientUnitService(IBaseUnitRepository baseUnitRepository,
            ICoefficientUnitRepository coefficientUnitRepository)
        {
            _baseUnitRepository = baseUnitRepository;
            _coefficientUnitRepository = coefficientUnitRepository;
        }
        public void CreateCoefficientUnit(string unitName, string metricName, string symbol, decimal conversionFactor, string baseUnitSymbol)
        {
            var coefficientUnit = _coefficientUnitRepository.Get(symbol).Result;

            if (coefficientUnit != null)
                if (coefficientUnit.Symbol == symbol)
                    throw new DuplicateSymbolException($"The unit {coefficientUnit.Symbol} is exist");

            BaseUnit baseUnit = _baseUnitRepository.Get(baseUnitSymbol).Result;
            if (baseUnit == null)
                throw new NotFoundException($"BaseUnit not found: {baseUnitSymbol}");

            if (baseUnit.MetricName != metricName)
                throw new IncompatibleMetricException($"CoefficientUnit is Incompatible with its BaseUnit");

            coefficientUnit = new()
            {
                UnitName = unitName,
                MetricName = metricName,
                Symbol = symbol,
                BaseUnit = baseUnit,
                ConversionFactor = conversionFactor
            };
            _coefficientUnitRepository.Add(coefficientUnit);
        }

        public decimal Convert(string fromUnit, string toUnit, decimal value)
        {
            decimal convertedToBaseValue;
            decimal fromConvertionrate = 1;
            decimal toConvertionrate = 1;
            var fromCoefficientUnit = _coefficientUnitRepository.Get(fromUnit).Result;
            if (fromCoefficientUnit == null)
            {
                BaseUnit baseUnit = _baseUnitRepository.Get(fromUnit).Result;
                if (baseUnit == null)
                    throw new NotFoundException($"Unit not found: {fromUnit}");
            }
            else
                fromConvertionrate = fromCoefficientUnit.ConversionFactor;

            decimal convertedValue;

            var toCoefficientUnit = _coefficientUnitRepository.Get(toUnit).Result;
            if (toCoefficientUnit == null)
            {
                BaseUnit baseUnit = _baseUnitRepository.Get(toUnit).Result;
                if (baseUnit == null)
                    throw new NotFoundException($"Unit not found: {toUnit}");
            }
            else
                toConvertionrate = toCoefficientUnit.ConversionFactor;


            if(fromConvertionrate > toConvertionrate)
            {
                convertedToBaseValue = value * fromConvertionrate;

                var convertionRate = 1 / toConvertionrate;
                convertedValue = convertionRate * convertedToBaseValue;

            }
            else
            {
                convertedToBaseValue = fromConvertionrate * value;

                var convertionRate = 1 / toConvertionrate;

                convertedValue = convertionRate * convertedToBaseValue;
            }

            return convertedValue;

        }
    }
}
