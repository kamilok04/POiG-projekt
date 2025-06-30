using Projekt.Miscellaneous;
using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // Sprawdź czy wydział i kierunek istnieją
                var checkFacultyQuery = "SELECT COUNT(*) as count FROM wydzial WHERE nazwa_krotka = @currentFaculty";
                var checkFacultyParams = new Dictionary<string, object>
                {
                    { "@currentFaculty", CurrentFaculty ?? string.Empty }
                };

                var facultyResult = await DatabaseHandler.ExecuteQueryAsync(checkFacultyQuery, checkFacultyParams);
                int facultyCount = 0;
                if (facultyResult?.Count > 0 && facultyResult[0].ContainsKey("count"))
                {
                    facultyCount = Convert.ToInt32(facultyResult[0]["count"]);
                }

                var checkDegreeQuery = "SELECT COUNT(*) as count FROM dane_kierunku WHERE nazwa = @currentDegree";
                var checkDegreeParams = new Dictionary<string, object>
                {
                    { "@currentDegree", CurrentDegree ?? string.Empty }
                };

                var degreeResult = await DatabaseHandler.ExecuteQueryAsync(checkDegreeQuery, checkDegreeParams);
                int degreeCount = 0;
                if (degreeResult?.Count > 0 && degreeResult[0].ContainsKey("count"))
                {
                    degreeCount = Convert.ToInt32(degreeResult[0]["count"]);
                }

                if (facultyCount == 0 || degreeCount == 0)
                {
                    return false;
                }

                // Pobierz ID wydziału i kierunku
                var getIdFacultyQuery = "SELECT id FROM wydzial WHERE nazwa_krotka = @currentFaculty";
                var getIdDegreeDetailsQuery = "SELECT id FROM dane_kierunku WHERE nazwa = @currentDegree";

                var facultyIdResult = await DatabaseHandler.ExecuteQueryAsync(getIdFacultyQuery, checkFacultyParams);
                var degreeIdResult = await DatabaseHandler.ExecuteQueryAsync(getIdDegreeDetailsQuery, checkDegreeParams);

                int facultyId = Convert.ToInt32(facultyIdResult?[0]["id"]);
                int degreeId = Convert.ToInt32(degreeIdResult?[0]["id"]);

                // Znajdź lub utwórz kierunek
                var findDegreeQuery = "SELECT id FROM kierunek WHERE id_wydzialu = @facultyId AND id_danych_kierunku = @degreeId";
                var findDegreeParams = new Dictionary<string, object>
                {
                    { "@facultyId", facultyId },
                    { "@degreeId", degreeId }
                };

                var existingDegreeResult = await DatabaseHandler.ExecuteQueryAsync(findDegreeQuery, findDegreeParams);
                int degreeIdFinal;

                if (existingDegreeResult?.Count > 0)
                {
                    degreeIdFinal = Convert.ToInt32(existingDegreeResult[0]["id"]);
                }
                else
                {
                    var insertDegreeQuery = "INSERT INTO kierunek (id_wydzialu, id_danych_kierunku) VALUES (@facultyId, @degreeId); SELECT LAST_INSERT_ID() as id;";
                    await DatabaseHandler.ExecuteQueryAsync(insertDegreeQuery, findDegreeParams);
                    var newDegreeResult = await DatabaseHandler.ExecuteQueryAsync(findDegreeQuery, findDegreeParams);
                    degreeIdFinal = Convert.ToInt32(newDegreeResult?[0]["id"]);
                }

                // Znajdź lub utwórz rocznik
                var findSemesterQuery = "SELECT id FROM rocznik WHERE id_kierunku = @degreeId AND semestr = @currentSemester";
                var findSemesterParams = new Dictionary<string, object>
                {
                    { "@degreeId", degreeIdFinal },
                    { "@currentSemester", CurrentSemester ?? string.Empty }
                };

                var existingSemesterResult = await DatabaseHandler.ExecuteQueryAsync(findSemesterQuery, findSemesterParams);
                int semesterIdFinal;

                if (existingSemesterResult?.Count > 0)
                {
                    semesterIdFinal = Convert.ToInt32(existingSemesterResult[0]["id"]);
                }
                else
                {
                    var insertSemesterQuery = "INSERT INTO rocznik (id_kierunku, semestr) VALUES (@degreeId, @currentSemester); SELECT LAST_INSERT_ID() as id;";
                    await DatabaseHandler.ExecuteQueryAsync(insertSemesterQuery, findSemesterParams);
                    var newSemesterResult = await DatabaseHandler.ExecuteQueryAsync(findSemesterQuery, findSemesterParams);
                    semesterIdFinal = Convert.ToInt32(newSemesterResult[0]["id"]);
                }

                // Aktualizuj grupę
                var updateGroupQuery = "UPDATE grupa SET numer = @groupnumber, id_rocznika = @semesterId WHERE id = @groupId";
                var updateGroupParams = new Dictionary<string, object>
                {
                    { "@groupnumber", GroupNumber ?? string.Empty },
                    { "@semesterId", semesterIdFinal },
                    { "@groupId", GroupId }
                };

                var updateCommand = DatabaseHandler.CreateCommand(updateGroupQuery, updateGroupParams);
                var updateSuccess = await DatabaseHandler.ExecuteInTransactionAsync(updateCommand);

                if (!updateSuccess)
                {
                    return false;
                }

                // Aktualizuj tabelę pośrednią
                var updateIntermediateQuery = "UPDATE grupa_student_rocznik SET id_rocznika = @semesterId WHERE id_grupy = @groupId";
                var updateIntermediateParams = new Dictionary<string, object>
                {
                    { "@semesterId", semesterIdFinal },
                    { "@groupId", GroupId }
                };

                var updateIntermediateCommand = DatabaseHandler.CreateCommand(updateIntermediateQuery, updateIntermediateParams);
                var updateIntermediateSuccess = await DatabaseHandler.ExecuteInTransactionAsync(updateIntermediateCommand);

                return updateIntermediateSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating group: {ex.Message}");
                return false;
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