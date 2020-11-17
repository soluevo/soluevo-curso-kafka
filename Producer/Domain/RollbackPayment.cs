using System;

namespace Producer.Domain
{
    public class RollbackPayment
    {
        public string IdPayment { get; set; }
        public string Reason { get; set; }    
    }
}