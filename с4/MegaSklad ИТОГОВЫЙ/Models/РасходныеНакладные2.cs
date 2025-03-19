using System;

namespace MegaSklad.Models
{
    public class РасходныеНакладные2
    {
        public Guid id { get; set; }
        public DateTime дата_отгрузки { get; set; }
        public string название_клиента { get; set; }
        public string название_склада { get; set; }
        public decimal общая_сумма { get; set; }
        public string примечания { get; set; }
    }
}