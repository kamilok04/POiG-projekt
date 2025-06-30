using Projekt.Miscellaneous;
using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public GroupCreateModel? _model;

        public string TableName => "groups";
        public string? DefaultQuery => BuildDefaultQuery(); // No default query for creation
        public Dictionary<string, object>? DefaultParameters => new() 
        {
            { "@groupnumber",GroupNumber ?? string.Empty},
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
            try
            {
                // sprawdź czy wydział i kierunek istnieją, jakie szukasz
                var checkFacultyQuery = "SELECT COUNT(*) as count FROM wydzial WHERE nazwa_krotka = @currentFaculty";
                var checkFacultyParams = new Dictionary<string, object>
                {
                    { "@currentFaculty", _currentFaculty ?? string.Empty }
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
                    { "@currentDegree", _currentDegree ?? string.Empty }
                };

                var degreeResult = await DatabaseHandler.ExecuteQueryAsync(checkDegreeQuery, checkDegreeParams);
                int degreeCount = 0;
                if (degreeResult?.Count > 0 && degreeResult[0].ContainsKey("count"))
                {
                    degreeCount = Convert.ToInt32(degreeResult[0]["count"]);
                }


                // jeśli wydział i kierunek istnieją, pozyskaj ich id
                if (facultyCount != 0 && degreeCount != 0)
                {
                    var getIdFacultyQuery = "SELECT id FROM wydzial WHERE nazwa_krotka = @currentFaculty";
                    var getIdDegreeDetailsQuery = "SELECT id FROM dane_kierunku WHERE nazwa = @currentDegree";

                    var facultyIdResult = await DatabaseHandler.ExecuteQueryAsync(getIdFacultyQuery, checkFacultyParams);
                    var degreeIdResult = await DatabaseHandler.ExecuteQueryAsync(getIdDegreeDetailsQuery, checkDegreeParams);

                    int facultyId = Convert.ToInt32(facultyIdResult?[0]["id"]);
                    int degreeId = Convert.ToInt32(degreeIdResult?[0]["id"]);

                    // i wprowadź do kierunku
                    var insertDegreeQuery = "INSERT INTO kierunek (id_wydzialu, id_danych_kierunku) VALUES (@facultyId, @degreeId); SELECT LAST_INSERT_ID() as id;";
                    var insertDegreeParams = new Dictionary<string, object>
                    {
                        { "@facultyId",facultyId },
                        { "@degreeId", degreeId }
                    };
                    await DatabaseHandler.ExecuteQueryAsync(insertDegreeQuery, insertDegreeParams);

                    // pobierz id kierunku aby wprowadzić do rocznika
                    var getIdDegreeQuery = "SELECT id FROM kierunek WHERE id_wydzialu = @facultyId AND id_danych_kierunku = @degreeId";
                    var resultDegreeId = await DatabaseHandler.ExecuteQueryAsync(getIdDegreeQuery, insertDegreeParams);
                    int degreeIdFinal = Convert.ToInt32(resultDegreeId?[0]["id"]);

                    var insertSemesterQuery = "INSERT INTO rocznik (id_kierunku, semestr) VALUES (@degreeId, @currentSemester); SELECT LAST_INSERT_ID() as id;";
                    var insertSemesterParams = new Dictionary<string, object>
                    {
                        { "@degreeId", degreeIdFinal },
                        { "@currentSemester", _currentSemester ?? string.Empty }
                    };

                    await DatabaseHandler.ExecuteQueryAsync(insertSemesterQuery, insertSemesterParams);

                    var getIdSemesterQuery = "SELECT id FROM rocznik WHERE id_kierunku = @degreeId AND semestr = @currentSemester";
                    var resultSemesterId = await DatabaseHandler.ExecuteQueryAsync(getIdSemesterQuery, insertSemesterParams);
                    int semesterIdFinal = Convert.ToInt32(resultSemesterId?[0]["id"]);

                    // na końcu do grupy wprowadź pełne zapytanie
                    var insertGroupQuery = "INSERT INTO grupa (numer, id_rocznika, podgrupa) VALUES (@groupnumber, @semesterId, NULL); SELECT LAST_INSERT_ID() as id;";
                    var insertGroupParams = new Dictionary<string, object>
                    {
                        { "@groupnumber", GroupNumber ?? string.Empty },
                        { "@semesterId", semesterIdFinal }
                    };

                    var groupResult = DatabaseHandler.CreateCommand(insertGroupQuery, insertGroupParams);
                    var groupSuccess = await DatabaseHandler.ExecuteInTransactionAsync(groupResult);

                    if (!groupSuccess)
                    {
                        return false;
                    }
                    // jak zadziałało, to wprowadź jeszcze dane do tablicy pośredniej grupa_student_rocznik
                    else
                    {
                        var insertToGroupStudentSemesterQuery = BuildDefaultQuery();
                        var insertToGroupStudentSemesterParams = new Dictionary<string, object>
                        {
                            { "@semesterId", semesterIdFinal },
                            { "@groupId", groupResult.LastInsertedId }
                        };

                        var insertToGroupStudentSemesterCommand = DatabaseHandler.CreateCommand(insertToGroupStudentSemesterQuery, insertToGroupStudentSemesterParams);
                        var insertToGroupStudentSemesterSuccess = await DatabaseHandler.ExecuteInTransactionAsync(insertToGroupStudentSemesterCommand);

                        if(!insertToGroupStudentSemesterSuccess)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding group: {ex.Message}");
                return false;
            }
        }

        public string BuildDefaultQuery()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("INSERT INTO grupa_student_rocznik VALUES (LAST_INSERT_ID(), @semesterId, @groupId)");
            return queryBuilder.ToString();
        }
    }
}
