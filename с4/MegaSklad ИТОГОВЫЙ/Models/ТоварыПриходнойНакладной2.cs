using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Товары_приходной_накладной")]
    public class ТоварыПриходнойНакладной2 : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid id { get; set; }

        [Column("id_приходной_накладной")]
        public Guid id_приходной_накладной { get; set; }

        [Column("id_товара")]
        public Guid id_товара { get; set; }

        [Column("количество")]
        public int количество { get; set; }

        [Column("цена")]
        public decimal цена { get; set; }
    }
}