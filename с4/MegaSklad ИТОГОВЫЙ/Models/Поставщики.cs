// Models/Поставщики.cs
using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Поставщики")]
    public class Поставщики : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("название_поставщика")]
        public string название_поставщика { get; set; }

        [Column("ИНН_поставщика")]
        public string ИНН_поставщика { get; set; }

        [Column("КПП_поставщика")]
        public string КПП_поставщика { get; set; }

        [Column("контактное_лицо")]
        public string контактное_лицо { get; set; }

        [Column("телефон_поставщика")]
        public string телефон_поставщика { get; set; }

        [Column("email_поставщика")]
        public string email_поставщика { get; set; }

        [Column("адрес_поставщика")]
        public string адрес_поставщика { get; set; }
    }
}