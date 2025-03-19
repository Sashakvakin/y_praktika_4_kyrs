using System;
using Postgrest.Attributes;
using Postgrest.Models;
using System.Threading.Tasks;

namespace MegaSklad.Models
{
    [Table("Товары")]
    public class Товары : BaseModel
    {
        [PrimaryKey("id")]
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
        public decimal? цена { get; set; }

        [Column("минимальный_остаток")]
        public int? минимальный_остаток { get; set; }

        [Column("описание")]
        public string описание { get; set; }


    }
}