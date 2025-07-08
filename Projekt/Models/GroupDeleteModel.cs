using Projekt.Miscellaneous;
using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class GroupDeleteModel : BaseTableModel, ITable
    {
        public int GroupId { get; set; }
        public string? GroupNumber { get; set; }
        public string? Faculty { get; set; }
        public string? Degree { get; set; }
        public string? Semester { get; set; }
        public int StudentCount { get; set; }

        public string TableName => "groups";
        public string? DefaultQuery => "CALL DeleteGroup(@groupId);";
        public Dictionary<string, object>? DefaultParameters => new()
        {
            { "@groupId", GroupId }
        };

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public GroupDeleteModel(LoginWrapper loginWrapper) : base(loginWrapper)
        {
        }

        public async Task<bool> LoadGroupData(int groupId)
        {
            try
            {
                GroupId = groupId;

                var query = @"SELECT g.id, g.numer, w.nazwa_krotka as wydział, dk.nazwa as kierunek, r.semestr,
                             (SELECT COUNT(*) FROM grupa_student_rocznik gsr WHERE gsr.id_grupy = g.id) as student_count
                             FROM grupa g
                             JOIN rocznik r ON g.id_rocznika = r.id
                             JOIN kierunek k ON r.id_kierunku = k.id
                             JOIN wydzial w ON k.id_wydzialu = w.id
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
                    Faculty = row["wydział"]?.ToString();
                    Degree = row["kierunek"]?.ToString();
                    Semester = row["semestr"]?.ToString();
                    StudentCount = Convert.ToInt32(row["student_count"] ?? 0);
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

        public async Task<bool> DeleteGroup()
        {
            try
            {
                var deleteGroupQuery = DefaultQuery;

                var deleteGroupParams = DefaultParameters;

                var deleteGroupCommand = DatabaseHandler.CreateCommand(deleteGroupQuery, deleteGroupParams);

                return await DatabaseHandler.ExecuteInTransactionAsync(deleteGroupCommand);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting group: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CanDeleteGroup()
        {
            try
            {
                // Sprawdź czy grupa ma przypisanych studentów
                var checkStudentsQuery = "SELECT COUNT(*) as count FROM grupa_student_rocznik WHERE id_grupy = @groupId";
                var checkStudentsParams = new Dictionary<string, object>
                {
                    { "@groupId", GroupId }
                };

                var result = await DatabaseHandler.ExecuteQueryAsync(checkStudentsQuery, checkStudentsParams);

                if (result?.Count > 0 && result[0].ContainsKey("count"))
                {
                    int studentCount = Convert.ToInt32(result[0]["count"]);
                    return studentCount == 0; // Można usunąć tylko jeśli brak studentów
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if group can be deleted: {ex.Message}");
                return false;
            }
        }

        public async Task<List<GroupDeleteModel>> GetAllGroupsAsync()
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

            var groups = new List<GroupDeleteModel>();

            foreach (var row in result)
            {
                groups.Add(new GroupDeleteModel(LoginWrapper)
                {
                    GroupId = Convert.ToInt32(row["GroupId"]),
                    GroupNumber = row["GroupNumber"]?.ToString(),
                    Faculty = row["Faculty"]?.ToString(),
                    Degree = row["Degree"]?.ToString(),
                    Semester = row["Semester"]?.ToString()
                });
            }

            return groups;
        }
    }
}