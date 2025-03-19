// Models/Склады.cs
using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace MegaSklad.Models
{
    [Table("Склады")]
    public class Склады : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("название_склада")]
        public string название_склада { get; set; }

        [Column("адрес_склада")]
        public string адрес_склада { get; set; }

        [Column("тип_склада")]
        public string тип_склада { get; set; }
    }
}