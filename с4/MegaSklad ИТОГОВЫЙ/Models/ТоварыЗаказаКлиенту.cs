using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Товары_заказа_клиенту")]
    public class ТоварыЗаказаКлиенту : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_заказа_клиенту")]
        public Guid id_заказа_клиенту { get; set; }

        [Column("id_товара")]
        public Guid id_товара { get; set; }

        [Column("количество")]
        public int количество { get; set; }
    }
}