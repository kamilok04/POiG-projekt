using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt.Models
{
    public class GroupCreateModel : BaseTableModel, ITable
    {
        public string? GroupNumber;
        public List<string>? _faculties;
        public List<string>? _degree;
        public List<int>? _semesters = [1, 2, 3, 4, 5, 6, 7];
        public string? _currentFaculty;
        public string? _currentDegree;
        public string? _currentSemester;
        public int? _subGroupNumber = 1; // Default value for sub-group number
        public GroupCreateModel? _model;

        public string TableName => "groups";
        public string? DefaultQuery => "CALL AddGroup(@groupNumber, @currentFaculty, @currentDegree, @currentSemester)"; // No default query for creation
        public Dictionary<string, object>? DefaultParameters => new()
        {
            { "@groupNumber",GroupNumber ?? string.Empty},
            { "@currentFaculty", _currentFaculty ?? string.Empty },
            { "@currentDegree", _currentDegree ?? string.Empty },
            { "@currentSemester", _currentSemester ?? string.Empty },
        };

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public GroupCreateModel(LoginWrapper loginWrapper) : base(loginWrapper)
        {
        }


        public async Task<bool> AddGroup()
        {
            // Transakcja: dodaj przedmiot
            var defaultQuery = ((ITable)this).DefaultQuery ?? throw new InvalidOperationException("DefaultQuery can't be null");
            var defaultParameters = ((ITable)this).DefaultParameters ?? throw new InvalidOperationException("DefaultParameters can't be null");

            MySqlCommand AddSubjectCommand = DatabaseHandler.CreateCommand(defaultQuery, defaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddSubjectCommand);
        }
    }
}