using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class TimeTableModel
    {
        private LoginWrapper Wrapper { get; init; }
        public TimeTableModel(LoginWrapper loginWrapper)
        {
            Wrapper = loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper));
        }

        public int[] StudentGroups { get; set; }
        public int[] CoordinatedGroups { get; set; }

        public async Task<Dictionary<string, List<LessonModel>>> GetTimeTable(List<string> daysOfWeek)
        {

            bool isCoordinator = false , isStudent = false;

            if (await Wrapper.IsUserACoordinator())
            {
                isCoordinator = true;
                CoordinatedGroups = (await GetCoordinatedGroups()).ToArray();
                
            }
            if(await Wrapper.IsUserAStudent())
            {
                isStudent = true;
                StudentGroups = (await GetStudentGroups()).ToArray();
            }
                Dictionary<string, List<LessonModel>> lessons = [];
            foreach (string day in daysOfWeek)
            {
                lessons[day] = [];
                if (isCoordinator)
                {
                    var todayLessons = await GetCoordinatorLessonsForDayAsync(day);
                    foreach (var lesson in todayLessons)
                    {
                        await lesson.LoadDataAsync(Wrapper);
                        lessons[day].Add(lesson);
                    }
                }
                if (isStudent) {
                    var todayLessons = await GetStudentLessonsForDayAsync(day);
                    foreach (var lesson in todayLessons)
                    {
                        await lesson.LoadDataAsync(Wrapper);
                        lessons[day].Add(lesson);
                    }
                }
            }
            return lessons;

        }

        

        /// <summary>
        /// Pobiera wszystkie zajęcia prowadzącego dla danego dnia tygodnia.
        /// </summary>
        /// <param name="day">Dzień tygodnia</param>
        /// <returns>IEnumerable<LessonModel> - lista lekcji</returns>
        private async Task<IEnumerable<LessonModel>> GetCoordinatorLessonsForDayAsync(string day)
        {
          
            var query = "SELECT * FROM zajecie WHERE dzien_tygodnia = @day AND FIND_IN_SET(id_grupy, @groupIds) <> 0";
            var parameters = new Dictionary<string, object>
            {
                { "@day", day },
                { "@groupIds", string.Join(",", CoordinatedGroups)}

            };
            var result = await Wrapper.DBHandler.ExecuteQueryAsync(query, parameters);
            if (result == null || result.Count == 0)
                return [];

            List<LessonModel> lessons = [];
            foreach (var row in result) {
                LessonModel lesson = new(row);
                lessons.Add(lesson);
                    }

            return lessons;


        }

        /// <summary>
        /// Pobiera wszystkie zajęcia studenta dla danego dnia tygodnia.
        /// </summary>
        /// <param name="day">Dzień tygodnia</param>
        /// <returns>IEnumerable<LessonModel> - lista lekcji</returns>
        private async Task<IEnumerable<LessonModel>> GetStudentLessonsForDayAsync(string day)
        {
           
            var query = "SELECT * FROM zajecie WHERE dzien_tygodnia = @day AND FIND_IN_SET(id_grupy, @groupIds) <> 0";
            var parameters = new Dictionary<string, object>
            {
                { "@day", day },
                { "@groupIds",  string.Join(",", StudentGroups) }
            };
            var result = await Wrapper.DBHandler.ExecuteQueryAsync(query, parameters);
            if (result == null || result.Count == 0)
                return [];

            List<LessonModel> lessons = [];
            foreach (var row in result)
            {
                LessonModel lesson = new(row);
                lessons.Add(lesson);
            }

            return lessons;


        }

        /// <summary>
        /// Pobiera grupy, które prowadzący koordynuje.
        /// </summary>
        /// <returns>IEnumerable<int> - lista identyfikatorów grup.</returns>
        /// <exception cref="ArgumentNullException">Nazwa użytkownika jest nieprawidłowa.</exception>
        private async Task<IEnumerable<int>> GetCoordinatedGroups()
        {
            if (Wrapper == null) throw new ArgumentNullException(nameof(Wrapper));
            if (Wrapper.DBHandler == null) throw new ArgumentNullException(nameof(Wrapper.DBHandler));
            var query = "SELECT id_grupy FROM grupa_przedmiot_prowadzacy WHERE id_prowadzacego = @coordinatorId";
            var parameters = new Dictionary<string, object>
            {
                { "@coordinatorId", Wrapper.Username }
            };
            var result = await Wrapper.DBHandler.ExecuteQueryAsync(query, parameters);
            if (result == null || result.Count == 0) return Array.Empty<int>();
            return result.Select(row => Convert.ToInt32(row["id_grupy"]));
        }

        /// <summary>
        /// Pobiera grupy, do których należy student.
        /// </summary>
        /// <returns>IEnumerable<int> - lista identyfikatorów grup.</returns>
        /// <exception cref="ArgumentNullException">Nazwa użytkownika jest nieprawidłowa.</exception>
        private async Task<IEnumerable<int>> GetStudentGroups()
        {
            if (Wrapper == null) throw new ArgumentNullException(nameof(Wrapper));
            if (Wrapper.DBHandler == null) throw new ArgumentNullException(nameof(Wrapper.DBHandler));
            var query = "SELECT id_grupy FROM grupa_student gs, student s WHERE gs.indeks = s.indeks AND s.login = @login";
            var parameters = new Dictionary<string, object>
            {
                { "@login", Wrapper.Username }
            };
            var result = await Wrapper.DBHandler.ExecuteQueryAsync(query, parameters);
            if (result == null || result.Count == 0) return Array.Empty<int>();
            return result.Select(row => Convert.ToInt32(row["id_grupy"]));
        }

    }
}
