using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Projekt.Models
{
    public class FacultyEditModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper)
    {
        DatabaseHandler DBHandler = loginWrapper.DBHandler;

        List<MySqlCommand> Commands = new List<MySqlCommand>();


        public void CreateTransactionCommand(string? columnName, string? RowKey, string? oldValue, string? newValue)
        {
            if (columnName == null || RowKey == null) return;
            MySqlCommand? command;
            string properColumnName = TranslateColumnName(columnName);
            if (oldValue == null || oldValue == "")
            {
                command = CreateInsertCommand(RowKey, newValue);
                if (command != null) Commands.Add(command);
            }
            if (newValue == null ||  newValue == "")
            {
                command = ConsiderDelete(properColumnName, RowKey);
                if (command != null) Commands.Add(command);
            }
            command = DatabaseHandler.CreateCommand($"UPDATE wydzial SET {properColumnName} = @value WHERE nazwa_krotka = @nazwa;",
                new() { { "@value", newValue }, { "@nazwa", oldValue } });

            Commands.Add(command);
            return;

        }

        private MySqlCommand? CreateInsertCommand(string key, string? value)
        => DatabaseHandler.CreateCommand("INSERT INTO wydzial (nazwa, nazwa_krotka) VALUES (@nazwa, @nazwa_krotka)",
                        new() { { "@nazwa_krotka", key }, { "@nazwa", value } });
     

        private MySqlCommand? ConsiderDelete(string col, string key)
        => DatabaseHandler.CreateCommand("DELETE FROM wydzial WHERE nazwa_krotka = @nazwa;",
                        new() { { "@nazwa", key } });

        public async Task<bool> CommitTransaction()
        {
            bool result =  await DBHandler.ExecuteInTransactionAsync([.. Commands]);
            Commands.Clear();
            return result;
        }

        public string TranslateColumnName(string columnName)
        {
            switch (columnName)
            {
                case "Nazwa": return "nazwa";
                case "Skrót": return "nazwa_krotka";
                default:
                    throw new ArgumentException($"Nieznana nazwa kolumny: {columnName}", nameof(columnName));
            }
        }
    }

}
