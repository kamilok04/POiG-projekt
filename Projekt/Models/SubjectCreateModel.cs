using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Projekt.Models
{
    public class SubjectCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string _code;
        public string _name;
        public string _description;
        public string _literature;
        public string passingCriteria;
        public int _points;
        public string _faculty;

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "subjects";

        // Czy to zapytanie jest poprawne?
        public string? DefaultQuery => BuildDefaultQuery();

        public Dictionary<string, object>? DefaultParameters => new()
            {
                { "@code",_code },
                { "@name", _name },
                { "@description", _description },
                { "@literature", _literature },
                { "@passingCriteria", passingCriteria },
                { "@points", _points },
                { "@faculty", _faculty },
            };

        public async Task<bool> AddSubject()
        {
            // Transakcja: dodaj przedmiot
            MySqlCommand AddSubjectCommand = DatabaseHandler.CreateCommand(((ITable)this).DefaultQuery, ((ITable)this).DefaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddSubjectCommand);
        }

        // TODO: Sprawdzić czy działa poprawnie
        private string BuildDefaultQuery()
        {
            StringBuilder mainQuery = new();

            StringBuilder insertDescriptionQuery = new();
            insertDescriptionQuery.Append("INSERT INTO opis VALUES (@description);");

            StringBuilder insertLiteratureQuery = new();
            insertLiteratureQuery.Append("INSERT INTO literatura VALUES (@literature);");

            StringBuilder insertPassingCriteriaQuery = new();
            insertPassingCriteriaQuery.Append("INSERT INTO literatura VALUES (@literature);");

            StringBuilder selectFacultyIdQuery = new();
            selectFacultyIdQuery.Append("SELECT id FROM wydzial WHERE nazwa=@faculty");

            StringBuilder selectDescriptionIdQuery = new();
            selectDescriptionIdQuery.Append("SELECT id FROM opis WHERE opis=@description");

            StringBuilder selectLiteratureIdQuery = new();
            selectLiteratureIdQuery.Append("SELECT id FROM literatura WHERE literatura=@literature");

            StringBuilder selectPassingCriteriaIdQuery = new();
            selectPassingCriteriaIdQuery.Append("SELECT id FROM opis WHERE warunki_zaliczenia=@passingCriteria");

            mainQuery.Append(insertDescriptionQuery)
                .Append(insertLiteratureQuery)
                .Append(insertPassingCriteriaQuery)
                .Append("INSERT INTO dane_przedmiotu (kod, nazwa, punkty)")
                .Append("VALUES (@code, @name, @passing_terms, @points);")
                .Append("INSERT INTO dane_przedmiotu (id_opisu, id_literatury, id_warunkow, wydzial_org)")
                .Append("SELECT opis.id, literatura.id, warunki_zaliczenia.id, wydzial.id")
                .Append("FROM opis, literatura, warunki_zaliczenia, wydzial")
                .Append($"WHERE opis.id={selectDescriptionIdQuery}")
                .Append($"AND literatura.id={selectLiteratureIdQuery}")
                .Append($"AND warunki_zaliczenia.id={selectPassingCriteriaIdQuery}")
                .Append($"AND wydzial.id={selectFacultyIdQuery};");

            return mainQuery.ToString();
        }
    }
}
