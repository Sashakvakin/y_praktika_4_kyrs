using System;
using System.Collections.Generic;
using System.Text;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MSM.Models
{
    [Table("Товары")] // Укажите имя вашей таблицы
    public class Товары : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid id { get; set; }

        [Column("название_товара")]
        public string название_товара { get; set; }

        [Column("артикул_товара")]
        public string артикул_товара { get; set; }

        [Column("штрихкод_товара")]
        public string штрихкод_товара { get; set; }

        [Column("id_категории")]
        public Guid id_категории { get; set; }

        [Column("единица_измерения")]
        public string единица_измерения { get; set; }

        [Column("цена")]
        public decimal цена { get; set; }

        [Column("минимальный_остаток")]
        public int минимальный_остаток { get; set; }

        [Column("описание")]
        public string описание { get; set; }

        public int количество { get; set; }
        //Добавлено для QR code
        public string qrCodeUrl { get; set; }
        // Добавьте другие свойства, соответствующие столбцам вашей таблицы
    }
}
