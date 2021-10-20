using SQLite;
using System;

namespace InventarioEstoque.Model
{
    public class Dados
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int Qtd { get; set; }
        public string Encontrado { get; set; }
        public string LeituraDigitado { get; set; }
        public DateTime DH_preenchimento { get; set; }
    }
}
