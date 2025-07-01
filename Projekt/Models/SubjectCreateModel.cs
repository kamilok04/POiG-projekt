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
        public string? _code;
        public string? _name;
        public string? _description;
        public string? _literature;
        public string? _passingCriteria;
        public uint? _points;
        public string? _currentFaculty;
        public List<string>? _faculties;

        private DatabaseHandler DatabaseHandler => LoginWrapper?.DBHandler ?? throw new InvalidOperationException("LoginWrapper or DBHandler is already null");

        public string TableName => "przedmiot";

        // Zapytanie jest poprawne, ale z jakiegoś powodu nie działa (testowałem bezpośrednio na bazie). Chyba trzeba je jakoś rozbić 
        public string? DefaultQuery => """
            INSERT INTO opis (opis) VALUES (@description);
            SET @`id_opisu` = LAST_INSERT_ID();

            INSERT INTO literatura (literatura) VALUES (@literature);
            SET @`id_literatury` = LAST_INSERT_ID();

            INSERT INTO warunki_zaliczenia (warunki_zaliczenia) VALUES (@passingCriteria);
            SET @`id_warunkow` = LAST_INSERT_ID();

            INSERT INTO dane_przedmiotu (kod, nazwa, id_opisu, id_literatury, id_warunkow, punkty, wydzial_org)
            VALUES (@code, @name, @`id_opisu`, @`id_literatury`, @`id_warunkow`, @points, @currentFaculty);

            SET @id_danych = LAST_INSERT_ID();
            INSERT INTO przedmiot (id_danych) VALUES (@id_danych);
            """;


        public Dictionary<string, object>? DefaultParameters => new()
            {
                { "@code",_code ?? string.Empty },
                { "@name", _name ?? string.Empty },
                { "@description", _description ?? string.Empty },
                { "@literature", _literature ?? string.Empty},
                { "@passingCriteria", _passingCriteria ?? string.Empty},
                { "@points", _points ?? 0},
                { "@currentFaculty", _currentFaculty ?? string.Empty },
            };

        public async Task<bool> AddSubject()
        {
            
            // Transakcja: dodaj przedmiot
            var defaultQuery = ((ITable)this).DefaultQuery ?? throw new InvalidOperationException("DefaultQuery can't be null");
            var defaultParameters = ((ITable)this).DefaultParameters ?? throw new InvalidOperationException("DefaultParameters can't be null");

            MySqlCommand AddSubjectCommand = DatabaseHandler.CreateCommand(defaultQuery, defaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddSubjectCommand);
        }
    }
}
