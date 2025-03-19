// Models/ЗаказПоставщику.cs
using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Заказы_поставщикам")]
    public class ЗаказПоставщику : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_поставщика")]
        public Guid id_поставщика { get; set; }

        [Column("дата_заказа")]
        public DateTime дата_заказа { get; set; }

        [Column("дата_ожидаемой_поставки")]
        public DateTime? дата_ожидаемой_поставки { get; set; } // nullable

        [Column("id_склада")]
        public Guid id_склада { get; set; }

        [Column("статус")]
        public string статус { get; set; }

        [Column("примечания")]
        public string примечания { get; set; }
    }
}