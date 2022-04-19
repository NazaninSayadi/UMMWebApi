namespace UMMDomain
{
    public class FormulatedUnit
    {
        public FormulatedUnit()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
        public BaseUnit BaseUnit { get; set; }
        public string MetricName { get; set; }
        public string UnitName { get; set; }
        public string Symbol { get; set; }
        public string? Formula { get; set; }
        
    }
}
