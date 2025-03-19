using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Инвентаризации")]
    public class Инвентаризации : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("id_склада")]
        public Guid id_склада { get; set; }

        [Column("дата_проведения")]
        public DateTime дата_проведения { get; set; }

        [Column("ответственный_пользователь_id")]
        public Guid ответственный_пользователь_id { get; set; }

        [Column("результат")]
        public string результат { get; set; }

        [Column("дата_начала")]
        public DateTime? дата_начала { get; set; }

        [Column("дата_окончания")]
        public DateTime? дата_окончания { get; set; }

        [Column("ссылка_на_документ")]
        public string ссылка_на_документ { get; set; }
    }
}