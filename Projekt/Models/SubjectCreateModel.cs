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

        public string TableName => "przedmiot";

        // Czy to zapytanie jest poprawne?
        public string? DefaultQuery => @"
            INSERT INTO opis (opis) VALUES (@description);
            SET @id_opisu = LAST_INSERT_ID();

            INSERT INTO literatura (literatura) VALUES (@literature);
            SET @id_literatury = LAST_INSERT_ID();

            INSERT INTO warunki_zaliczenia (warunki_zaliczenia) VALUES (@passingCriteria);
            SET @id_warunkow = LAST_INSERT_ID();

            SET @wydzial_id = (SELECT id FROM wydzial WHERE nazwa = @faculty);

            INSERT INTO dane_przedmiotu (kod, nazwa, id_opisu, id_literatury, id_warunkow, punkty, wydzial_org)
            VALUES (@code, @name, @id_opisu, @id_literatury, @id_warunkow, @points, @wydzial_id);

            SET @id_danych = LAST_INSERT_ID();
            INSERT INTO przedmiot (id_danych) VALUES (@id_danych);
            ";


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
    }
}
