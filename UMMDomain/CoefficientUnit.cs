namespace UMMDomain
{
    public class CoefficientUnit
    {
        public CoefficientUnit()
        {
            Id = new Guid();
        }
        public Guid Id { get; private set; }
        public BaseUnit BaseUnit { get; set; }
        public string MetricName { get; set; }
        public string UnitName { get; set; }
        public string Symbol { get; set; }
        public decimal ConversionFactor { get; set; }
    }
}
