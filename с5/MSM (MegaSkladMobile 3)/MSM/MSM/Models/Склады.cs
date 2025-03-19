using System;
using System.Collections.Generic;
using System.Text;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MSM.Models
{
    [Table("Склады")] // Укажите имя вашей таблицы
    public class Склады : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid id { get; set; }

        [Column("название_склада")]
        public string название_склада { get; set; }

        [Column("адрес_склада")]
        public string адрес_склада { get; set; }

        [Column("тип_склада")]
        public string тип_склада { get; set; }

    }
}