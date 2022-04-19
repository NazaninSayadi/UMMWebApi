namespace UMMBusinessLogic.Services
{
    public interface IFormulatedUnitService
    {
        void CreateFormulatedUnit(string unitName, string metricName, string symbol, string formula, string baseUnit);
    }
}
