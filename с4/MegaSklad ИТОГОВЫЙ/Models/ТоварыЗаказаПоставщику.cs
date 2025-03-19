using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Товары_заказа_поставщику")]
    public class ТоварыЗаказаПоставщику : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_заказа_поставщику")]
        public Guid id_заказа_поставщику { get; set; }

        [Column("id_товара")]
        public Guid id_товара { get; set; }

        [Column("количество")]
        public int количество { get; set; }
    }
}