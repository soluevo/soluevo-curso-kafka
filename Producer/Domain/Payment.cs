using System;

namespace Producer.Domain
{
    public class Payment
    {
        public string Id { get; }
        public double Value { get; set; }
        public string Description { get; set; }
        public string SourceAccount { get; set; }
        public string TargetAccount { get; set; }

        public Payment()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}