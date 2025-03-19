// Models/Пользователи.cs
using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MegaSklad.Models
{
    [Table("Пользователи")]
    public class Пользователи : BaseModel
    {
        [PrimaryKey("id")]
        public string id { get; set; }

        [Column("supabase_auth_id")]
        public string supabase_auth_id { get; set; }

        [Column("имя_пользователя")]
        public string имя_пользователя { get; set; }

        [Column("должность")]
        public string должность { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Column("телефон")]
        public string телефон { get; set; }

        [Column("роль")]
        public string роль { get; set; }

        [Column("фото")]
        public Guid? фото { get; set; }
    }
}