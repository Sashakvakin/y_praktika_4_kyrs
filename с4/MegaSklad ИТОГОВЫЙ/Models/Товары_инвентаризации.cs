using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Товары_инвентаризации")]
    public class Товары_инвентаризации : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_инвентаризации")]
        public Guid id_инвентаризации { get; set; }

        [Column("id_товара")]
        public Guid id_товара { get; set; }

        [Column("фактическое_количество")]
        public int? фактическое_количество { get; set; }

        [Column("ожидаемое_количество")]
        public int? ожидаемое_количество { get; set; }

        [Column("расхождение")]
        public int? расхождение { get; set; }

        [Column("примечания")]
        public string примечания { get; set; }
    }
}