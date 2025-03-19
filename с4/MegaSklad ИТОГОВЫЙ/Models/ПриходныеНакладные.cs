//Models/ПриходныеНакладные.cs
using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace MegaSklad.Models
{
    [Table("Приходные_накладные")]
    public class ПриходныеНакладные : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_поставщика")]
        public Guid id_поставщика { get; set; }

        [Column("id_склада")]
        public Guid id_склада { get; set; }

        [Column("дата_поступления")]
        public DateTime дата_поступления { get; set; }

        [Column("общая_сумма")]
        public decimal общая_сумма { get; set; }

        [Column("примечания")]
        public string примечания { get; set; }
    }
}