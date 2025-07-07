using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class GroupStudentModel : BaseTableModel
    {
        /// <summary>
        /// Inicjalizuje nową instancję klasy GroupStudentModel z podanym wrapperem logowania.
        /// </summary>
        public GroupStudentModel(LoginWrapper wrapper) : base(wrapper)
        {
            LoginWrapper = wrapper;
        }

        /// <summary>
        /// Pobiera listę studentów przypisanych do określonej grupy na podstawie identyfikatora grupy.
        /// </summary>
        /// <param name="groupId">Identyfikator grupy.</param>
        /// <returns>Lista obiektów ExtendedStudentModel należących do grupy.</returns>
        public async Task<List<ExtendedStudentModel>> GetCurrentGroupStudents(int groupId)
        {
            if (LoginWrapper == null)
                throw new InvalidOperationException("LoginWrapper is not initialized.");
            if (LoginWrapper.DBHandler == null)
                throw new InvalidOperationException("DBHandler is not initialized in LoginWrapper.");

            string studentIdQuery = "SELECT indeks FROM grupa_student WHERE id_grupy = @groupId";
            var parameters = new Dictionary<string, object>
                {
                    { "@groupId", groupId }
                };
            try
            {
                var studentIds = await LoginWrapper.DBHandler.ExecuteQueryAsync(studentIdQuery, parameters);
                List<ExtendedStudentModel> result = new();
                foreach (var row in studentIds)
                {
                    var student = await RetrieveService.GetAsync<ExtendedStudentModel>(LoginWrapper, row["indeks"]);
                    if (student != null)
                    {
                        result.Add(student);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Nie udało się pobrać studentów należących do grupy", ex);
            }
        }

        /// <summary>
        /// Pobiera listę studentów spełniających określone kryteria filtrowania (wydział, semestr, kierunek).
        /// </summary>
        /// <param name="faculty">Model wydziału (może być null).</param>
        /// <param name="semester">Numer semestru (może być null).</param>
        /// <param name="major">Model kierunku (może być null).</param>
        /// <returns>Lista obiektów ExtendedStudentModel spełniających kryteria.</returns>
        public async Task<List<ExtendedStudentModel>> GetFilteredStudents(FacultyModel? faculty, int? semester, MajorModel? major)
        {
            if (LoginWrapper == null)
                throw new InvalidOperationException("LoginWrapper is not initialized.");
            if (LoginWrapper.DBHandler == null)
                throw new InvalidOperationException("DBHandler is not initialized in LoginWrapper.");

            var parameters = new Dictionary<string, object>();
            if (faculty != null)
                parameters["@facultyName"] = faculty.Name;
            if (major != null && major.Name != null)
                parameters["@majorName"] = major.Name;
            if (semester > 0)
                parameters["@semester"] = semester;

            var queryBuilder = new StringBuilder("SELECT * FROM dane_studenta");
            var whereClauses = new List<string>();
            if (parameters.ContainsKey("@facultyName"))
                whereClauses.Add("Wydział = @facultyName");
            if (parameters.ContainsKey("@majorName"))
                whereClauses.Add("Kierunek = @majorName");
            if (parameters.ContainsKey("@semester"))
                whereClauses.Add("Semestr = @semester");
            if (whereClauses.Count > 0)
                queryBuilder.Append(" WHERE " + string.Join(" AND ", whereClauses));

            var students = await LoginWrapper.DBHandler.ExecuteQueryAsync(queryBuilder.ToString(), parameters);

            var studentModels = students
                .Select(item => new ExtendedStudentModel(item)).ToList();
            return studentModels;
        }

        /// <summary>
        /// Tworzy polecenia SQL do dodania i usunięcia przypisań studentów do grupy.
        /// </summary>
        /// <param name="toAdd">Lista identyfikatorów studentów do dodania.</param>
        /// <param name="toRemove">Lista identyfikatorów studentów do usunięcia.</param>
        /// <param name="groupId">Identyfikator grupy.</param>
        /// <returns>Tablica poleceń MySqlCommand do wykonania w transakcji.</returns>
        public MySqlCommand[] CreateGroupTransactionCommands(List<int> toAdd, List<int> toRemove, int groupId)
        {
            List<MySqlCommand> commands = [];
            foreach (var id in toAdd)
            {
                commands.Add(DatabaseHandler.CreateCommand(
                    "INSERT INTO grupa_student (indeks, id_grupy) VALUES (@studentID, @groupID);",
                    new() { { "@studentID", id }, { "@groupID", groupId }
                    }));

            }

            foreach (var id in toRemove)
            {
                commands.Add(DatabaseHandler.CreateCommand(
                    "DELETE FROM grupa_student WHERE indeks = @studentID AND id_grupy = @groupID;",
                     new() { { "@studentID", id }, { "@groupID", groupId }
                    }));
            }

            return commands.ToArray();
        }

        /// <summary>
        /// Wykonuje przekazane polecenia SQL w ramach jednej transakcji.
        /// </summary>
        /// <param name="commands">Tablica poleceń MySqlCommand do wykonania.</param>
        /// <returns>True, jeśli transakcja zakończyła się sukcesem; w przeciwnym razie false.</returns>
        public async Task<bool> ExecuteAssignments(MySqlCommand[] commands)
        {
            if (LoginWrapper == null)
                throw new InvalidOperationException("LoginWrapper is not initialized.");
            if (LoginWrapper.DBHandler == null)
                throw new InvalidOperationException("DBHandler is not initialized in LoginWrapper.");

            return await LoginWrapper.DBHandler.ExecuteInTransactionAsync(commands);
        }

        /// <summary>
        /// Pobiera listę kierunków przypisanych do określonego wydziału.
        /// </summary>
        /// <param name="facultyName">Nazwa wydziału.</param>
        /// <returns>Lista obiektów MajorModel przypisanych do wydziału.</returns>
        public async Task<List<MajorModel>> GetFilteredMajors(string facultyName)
        {
            if (LoginWrapper == null)
                throw new InvalidOperationException("LoginWrapper is not initialized.");
            if (LoginWrapper.DBHandler == null)
                throw new InvalidOperationException("DBHandler is not initialized in LoginWrapper.");

            var results = await LoginWrapper.DBHandler.ExecuteQueryAsync("SELECT * FROM kierunek WHERE id_wydzialu = @facultyName",
                new Dictionary<string, object> { { "@facultyName", facultyName } });
            return [.. results.Select(item => new MajorModel(item, LoginWrapper))];
        }
    }
}
