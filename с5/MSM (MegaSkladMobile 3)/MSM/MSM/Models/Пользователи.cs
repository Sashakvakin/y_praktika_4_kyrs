using System;
using System.Collections.Generic;
using System.Text;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MSM.Models
{
    [Table("Пользователи")] // Укажите имя вашей таблицы
    public class Пользователи : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid id { get; set; }

        [Column("имя_пользователя")]
        public string имя_пользователя { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Column("телефон")]
        public string телефон { get; set; }

        [Column("роль")]
        public string роль { get; set; }

        // Добавьте другие свойства, соответствующие столбцам вашей таблицы
    }
}