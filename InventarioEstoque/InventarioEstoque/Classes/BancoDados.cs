using System.Timers;
using System.Collections.Generic;
using System.IO;
using InventarioEstoque.Model;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System;

namespace InventarioEstoque.Classes
{
    class BancoDados
    {
        public static void cria_banco_de_dados()
        {
            if (!File.Exists(Path.Combine(Constantes.path, "Dados.db")))
            {
                using (var conexao = new SQLiteConnection(Path.Combine(Constantes.path, "Dados.db")))
                {
                    conexao.CreateTable<Dados>();
                }
            }
            else
            {
                Log.Warning("Alerta", "O banco de dados já existe");
            }
        }

        public static bool insere_dados_no_banco(Dados dados)
        {
            try
            {
                using (var conexao = new SQLiteConnection(Path.Combine(Constantes.path, "Dados.db")))
                {
                    conexao.Insert(dados);
                    return true;
                }

            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLiteError", ex.Message);
                return false;
            }
        }

        public static List<Dados> le_dados_do_banco()
        {
            try
            {
                using (var conexao = new SQLiteConnection(Path.Combine(Constantes.path, "Dados.db")))
                {
                    return conexao.Table<Dados>().ToList();
                }
            }
            catch (SQLiteException)
            {
                return null;
            }
        }

        public static bool atualizar_dados(Dados dados)
        {
            try
            {
                using (var conexao = new SQLiteConnection(Path.Combine(Constantes.path, "Dados.db")))
                {
                    conexao.Query<Dados>("UPDATE Dados set Qtd = ?, LeituraDigitado = ?,Encontrado = ?, DH_preenchimento = ? Where Codigo = ?", dados.Qtd, dados.LeituraDigitado, dados.Encontrado, dados.DH_preenchimento, dados.Codigo);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLiteError", ex.Message);
                return false;
            }
        }

        public static bool limpa_dados(Dados dados)
        {
            try
            {
                using (var conexao = new SQLiteConnection(Path.Combine(Constantes.path, "Dados.db")))
                {
                    conexao.Query<Dados>("UPDATE Dados set Qtd = ?, LeituraDigitado = ?,Encontrado = ?, Where Codigo = ?", 0, "", "", dados.Codigo);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLiteError", ex.Message);
                return false;
            }
        }

        public static bool deletar_dados(Dados dados)
        {
            try
            {
                using (var conexao = new SQLiteConnection(Path.Combine(Constantes.path, "Dados.db")))
                {
                    conexao.Delete<Dados>(dados.id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLiteError", ex.Message);
                return false;
            }
        }
    }
}
