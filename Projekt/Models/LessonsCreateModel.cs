using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class LessonsCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string? _dayOfWeek; 
        public string? _startTime;
        public string? _endTime;
        public int? _subjectId;
        public int? _groupId;
        public int? _placeId;

        public string TableName => "lessons"; 

        public string? DefaultQuery => @"
            INSERT INTO zajecie (dzien_tygodnia, godzina_start, godzina_stop, id_przedmiotu, id_grupy, id_miejsca)
            VALUES (@dzien_tygodnia, @godzina_start, @godzina_stop, @id_przedmiotu, @id_grupy, @id_miejsca)"; 

        public Dictionary<string, object>? DefaultParameters => new()
        {
            { "@dzien_tygodnia", _dayOfWeek ?? String.Empty },
            { "@godzina_start", _startTime ?? String.Empty },
            { "@godzina_stop", _endTime ?? String.Empty },
            { "@id_przedmiotu", _subjectId ?? 0 },
            { "@id_grupy", _groupId ?? 0 },
            { "@id_miejsca", _placeId ?? 0 }
        }; 

        private DatabaseHandler DatabaseHandler => LoginWrapper?.DBHandler ?? throw new InvalidOperationException("LoginWrapper or DBHandler is already null");

        public async Task<bool> AddLesson()
        {

            // Transakcja: dodaj zajęcia
            var defaultQuery = ((ITable)this).DefaultQuery ?? throw new InvalidOperationException("DefaultQuery can't be null");
            var defaultParameters = ((ITable)this).DefaultParameters ?? throw new InvalidOperationException("DefaultParameters can't be null");

            MySqlCommand AddLessonCommand = DatabaseHandler.CreateCommand(defaultQuery, defaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddLessonCommand);
        }
    }
}
