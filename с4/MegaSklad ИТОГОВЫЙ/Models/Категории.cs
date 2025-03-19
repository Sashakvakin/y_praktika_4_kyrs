// Models/Категории.cs
using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Категории")]
    public class Категории : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("название_категории")]
        public string название_категории { get; set; }
    }
}