using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;

namespace Projekt.Models
{
    public class DegreeCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string Faculty = "";
        public string Name = "";

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "kierunek";

        public string? DefaultQuery => "CALL AddDegree(@name, @faculty);";

        public Dictionary<string, object>? DefaultParameters => new()
            {

                { "@name", Name },
                { "@faculty", Faculty }
            };

        public async Task<bool> AddDegree()
        {
            // Transakcja: dodaj kierunek
            MySqlCommand AddDegreeCommand = DatabaseHandler.CreateCommand(((ITable)this).DefaultQuery, ((ITable)this).DefaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddDegreeCommand);
        }

    }
}
