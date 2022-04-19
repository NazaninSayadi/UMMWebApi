namespace UMMBusinessLogic.Services
{
    public interface ICoefficientUnitService
    {
        void CreateCoefficientUnit(string unitName, string metricName, string symbol, decimal conversionFactor, string baseUnit);
        decimal Convert(string fromUnit, string toUnit, decimal value);
    }
}
