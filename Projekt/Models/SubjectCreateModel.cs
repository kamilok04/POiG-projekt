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

        // Procedura w SQL do tworzenia przedmiotów 
        public string? DefaultQuery => "CALL AddSubject(@description, @literature, @passingCriteria, @code, @name, @points, @currentFaculty);";


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
