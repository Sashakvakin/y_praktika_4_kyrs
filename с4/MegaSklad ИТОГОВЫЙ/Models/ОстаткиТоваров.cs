using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Остатки_товаров")]
    public class ОстаткиТоваров : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_склада")]
        public Guid id_склада { get; set; }

        [Column("id_товара")]
        public Guid id_товара { get; set; }

        [Column("количество")]
        public int количество { get; set; }

        [Column("дата_обновления")]
        public DateTime дата_обновления { get; set; }
    }
}