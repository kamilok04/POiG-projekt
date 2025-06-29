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
    public class UsersEditModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper)
    {
        DatabaseHandler DBHandler = loginWrapper.DBHandler;

        List<MySqlCommand> Commands = new List<MySqlCommand>();


        private string DetermineTableName(string columnName)
        {
            switch (columnName)
            {
                case "indeks": return "student";
                case "tytul": return "prowadzacy";
                default: return "uzytkownik";
            };
        }

        public void CreateTransactionCommand(string? columnName, string? RowKey, string? oldValue, string? newValue)
        {
            if (columnName == null || RowKey == null) return;
            MySqlCommand? command;
            string properColumnName = TranslateColumnName(columnName);
            if (oldValue == null)
            {
                command = CreateInsertCommand(properColumnName, RowKey, newValue);
                if (command != null) Commands.Add(command);
            }
            if (newValue == null)
            {
                command = ConsiderDelete(properColumnName, RowKey);
                if (command != null) Commands.Add(command);
            }
            command = DatabaseHandler.CreateCommand($"UPDATE {DetermineTableName(properColumnName)} SET {properColumnName} = @value WHERE login = @login;",
                new() { { "@value", newValue }, { "@login", RowKey } });
            // nie ma mowy o SQL Injection, użytkownik nie ma dostępu do nazw kolumn
            Commands.Add(command);
            return;

        }

        private MySqlCommand? CreateInsertCommand(string col, string key, string? value)
        {
            switch (col)
            {
                case "indeks":
                    return DatabaseHandler.CreateCommand("INSERT INTO student (login, indeks) VALUES (@login, @indeks)",
                        new() { { "@login", key }, { "@indeks", value } });
                case "tytul":
                    return DatabaseHandler.CreateCommand("INSERT INTO prowadzacy (login, tytul) VALUES (@login, @tytul)",
                         new() { { "@login", key }, { "@tytul", value } });
                default: return null;
            }
        }

        private MySqlCommand? ConsiderDelete(string col, string key)
        {
            switch (col)
            {
                case "indeks":
                    return DatabaseHandler.CreateCommand("DELETE FROM student WHERE login = @login;",
                        new() { { "@login", key } });
                case "tytul":
                    return DatabaseHandler.CreateCommand("DELETE FROM prowadzacy WHERE login = @login;",
                         new() { { "@login", key } });
                default: return null;
            }
        }

        public async Task<bool> CommitTransaction()
        {
            return await DBHandler.ExecuteInTransactionAsync([.. Commands]);
        }

        public string TranslateColumnName(string columnName)
        {
            switch (columnName)
            {
                case "Login": return "login";
                case "Data urodzenia": return "data_urodzenia";
                case "Imię":
                    return "imie";
                case "Nazwisko":
                    return "Nazwisko";
                case "Adres e-mail": return "email";
                case "Uprawnienia": return "uprawnienia";
                case "Nr indeksu": return "indeks";
                case "Tytuł naukowy": return "tytul";
                default: return String.Empty;
            }
        }
    }

}
