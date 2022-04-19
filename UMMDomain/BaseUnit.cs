namespace UMMDomain
{
    public class BaseUnit
    {
        public BaseUnit()
        {
            Id = new Guid();
        }
        public Guid Id { get; private set; }
        public string MetricName { get; set; }
        public string UnitName { get; set; }
        public string Symbol { get; set; }

    }
}
