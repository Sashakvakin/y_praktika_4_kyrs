// Models/ЗаказКлиенту.cs
using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Заказы_клиентам")]
    public class ЗаказКлиенту : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_клиента")]
        public Guid id_клиента { get; set; }

        [Column("дата_заказа")]
        public DateTime дата_заказа { get; set; }

        [Column("дата_ожидаемой_отгрузки")]
        public DateTime? дата_ожидаемой_отгрузки { get; set; } // nullable

        [Column("id_склада")]
        public Guid id_склада { get; set; }

        [Column("статус")]
        public string статус { get; set; }

        [Column("примечания")]
        public string примечания { get; set; }
    }
}