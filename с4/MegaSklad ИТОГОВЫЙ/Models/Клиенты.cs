// Models/Клиенты.cs
using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Клиенты")]
    public class Клиенты : BaseModel
    {
        [PrimaryKey("id")]
        public Guid id { get; set; }

        [Column("название_клиента")]
        public string название_клиента { get; set; }

        [Column("контактное_лицо")]
        public string контактное_лицо { get; set; }

        [Column("телефон_клиента")]
        public string телефон_клиента { get; set; }

        [Column("email_клиента")]
        public string email_клиента { get; set; }

        [Column("адрес_клиента")]
        public string адрес_клиента { get; set; }
    }
}