using System;
using Supabase.Realtime.Models;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MSM.Models
{
    [Table("Остатки_товаров")] // Укажите имя вашей таблицы
    public class ОстаткиТоваров : BaseModel
    {
        [PrimaryKey("id", false)]
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