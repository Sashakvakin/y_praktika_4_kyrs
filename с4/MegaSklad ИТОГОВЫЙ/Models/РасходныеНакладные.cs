using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace MegaSklad.Models
{
    [Table("Расходные_накладные")]
    public class РасходныеНакладные : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_клиента")]
        public Guid id_клиента { get; set; }

        [Column("id_склада")]
        public Guid id_склада { get; set; }

        [Column("дата_отгрузки")]
        public DateTime дата_отгрузки { get; set; }

        [Column("общая_сумма")]
        public decimal общая_сумма { get; set; }

        [Column("примечания")]
        public string примечания { get; set; }
    }
}