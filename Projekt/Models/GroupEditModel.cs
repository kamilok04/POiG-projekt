using Projekt.Miscellaneous;
using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Projekt.Models
{
    public class GroupEditModel : BaseTableModel, ITable
    {
        public int GroupId { get; set; }
        public string? GroupNumber { get; set; }
        public List<string>? _faculties;
        public List<string>? _degree;
        public List<int>? _semesters = [1, 2, 3, 4, 5, 6, 7];
        public string? CurrentFaculty;
        public string? CurrentDegree;
        public string? CurrentSemester;
        public GroupEditModel? _model;

        public string TableName => "groups";
        public string? DefaultQuery => BuildDefaultQuery();
        public Dictionary<string, object>? DefaultParameters => new()
        {
            { "@groupId", GroupId },
            { "@groupnumber", GroupNumber ?? string.Empty },
            { "@currentFaculty", CurrentFaculty ?? string.Empty },
            { "@currentDegree", CurrentDegree ?? string.Empty },
            { "@currentSemester", CurrentSemester ?? string.Empty },
        };

        public override string ToString()
        {
            return $"{CurrentFaculty}>{CurrentDegree}>{CurrentSemester}/{GroupNumber}";
        }

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public GroupEditModel(LoginWrapper loginWrapper) : base(loginWrapper)
        {
        }

        public async Task<bool> LoadGroupData(int groupId)
        {
            try
            {
                GroupId = groupId;

                var query = @"SELECT g.numer, w.nazwa_krotka as wydział, dk.nazwa as kierunek, r.semestr 
                             FROM grupa g
                             JOIN rocznik r ON g.id_rocznika = r.id
                             JOIN kierunek k ON r.id_kierunku = k.id
                             JOIN wydzial w ON k.id_wydzialu = w.nazwa_krotka
                             JOIN dane_kierunku dk ON k.id_danych_kierunku = dk.id
                             WHERE g.id = @groupId";

                var parameters = new Dictionary<string, object>
                {
                    { "@groupId", groupId }
                };

                var result = await DatabaseHandler.ExecuteQueryAsync(query, parameters);

                if (result?.Count > 0)
                {
                    var row = result[0];
                    GroupNumber = row["numer"]?.ToString();
                    CurrentFaculty = row["wydział"]?.ToString();
                    CurrentDegree = row["kierunek"]?.ToString();
                    CurrentSemester = row["semestr"]?.ToString();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading group data: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateGroup()
        {
            try
            {
                var updateSubjectQuery = "CALL UpdateGroup(@groupId, @groupnumber, @currentFaculty, @currentDegree, @currentSemester)";

                var updateSubjectParams = DefaultParameters;

                var updateSubjectCommand = DatabaseHandler.CreateCommand(updateSubjectQuery, updateSubjectParams);

                return await DatabaseHandler.ExecuteInTransactionAsync(updateSubjectCommand);
            }
            catch (Exception)
            {
                return false; // Return false on any error
            }
        }

        public async Task<List<GroupEditModel>> GetAllGroupsAsync()
        {
            var query = @"
        SELECT g.id as GroupId, g.numer as GroupNumber, w.nazwa_krotka as Faculty, dk.nazwa as Degree, r.semestr as Semester
        FROM grupa g
        JOIN rocznik r ON g.id_rocznika = r.id
        JOIN kierunek k ON r.id_kierunku = k.id
        JOIN wydzial w ON k.id_wydzialu = w.nazwa_krotka
        JOIN dane_kierunku dk ON k.id_danych_kierunku = dk.id;
    ";

            var result = await DatabaseHandler.ExecuteQueryAsync(query);

            var groups = new List<GroupEditModel>();

            foreach (var row in result)
            {
                groups.Add(new GroupEditModel(LoginWrapper)
                {
                    GroupId = Convert.ToInt32(row["GroupId"]),
                    GroupNumber = row["GroupNumber"]?.ToString(),
                    CurrentFaculty = row["Faculty"]?.ToString(),
                    CurrentDegree = row["Degree"]?.ToString(),
                    CurrentSemester = row["Semester"]?.ToString()
                });
            }

            return groups;
        }

        public string BuildDefaultQuery()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("UPDATE grupa SET numer = @groupnumber, id_rocznika = (SELECT id FROM rocznik WHERE id_kierunku = (SELECT id FROM kierunek WHERE id_wydzialu = (SELECT id FROM wydzial WHERE nazwa_krotka = @currentFaculty) AND id_danych_kierunku = (SELECT id FROM dane_kierunku WHERE nazwa = @currentDegree)) AND semestr = @currentSemester) WHERE id = @groupId");
            return queryBuilder.ToString();
        }
    }
}