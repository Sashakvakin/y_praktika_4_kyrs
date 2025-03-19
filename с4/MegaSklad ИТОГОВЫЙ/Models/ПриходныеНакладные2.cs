using System;

namespace MegaSklad.Models
{
    public class ПриходныеНакладные2
    {
        public Guid id { get; set; }
        public DateTime дата_поступления { get; set; }
        public string название_поставщика { get; set; }
        public string название_склада { get; set; }
        public decimal общая_сумма { get; set; }
        public string примечания { get; set; }
    }
}