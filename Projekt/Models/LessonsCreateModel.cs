using Org.BouncyCastle.Asn1.X509.Qualified;
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
        public string TableName => "lessons";
        public string? DefaultQuery => String.Empty;  
        public Dictionary<string, object>? DefaultParameters => null; 

        private async Task<List<SubjectModel>?> GetAllSubjectsAsync()
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT przedmiot.id, id_danych, kod, nazwa, id_opisu, id_literatury, id_warunkow, punkty, wydzial_org " +
                "FROM przedmiot " +
                "JOIN dane_przedmiotu " +
                "ON dane_przedmiotu.id = przedmiot.id_danych");
            var subjects = new List<SubjectModel>();
            foreach (var row in result)
            {
                var subject  = new SubjectModel(row);
                subjects.Add(subject);
            }
            return subjects;
        }

        private async Task<List<PlaceModel>?> GetAllPlacesAsync()
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
              "SELECT id, id_wydzialu, id_adresu, numer, pojemnosc " +
              "FROM miejsce;");
            return result.Select(row => new PlaceModel(row)).ToList();
        }

        private async Task<List<GroupEditModel>?> GetAllGroupsAsync()
        {
            var model = new GroupEditModel(loginWrapper);
            return await model.GetAllGroupsAsync();
        }

        public async Task<List<T>?> GetAllAsync<T>() where T : class
        {
            if (typeof(T) == typeof(SubjectModel))
            {
             var subjects = await GetAllSubjectsAsync();
                return subjects as List<T>;
            }
            else if (typeof(T) == typeof(PlaceModel))
            {
                var places = await GetAllPlacesAsync();
                return places as List<T>;
            }
            else if (typeof(T) == typeof(GroupEditModel))
            {
                var places = await GetAllGroupsAsync();
                return places as List<T>;
            }

            return null; 
        }

        public async Task<bool> SaveAsync(GroupEditModel? group, SubjectModel? subject, PlaceModel? place, string? type, string? dayOfWeek, TimeOnly? start, TimeOnly? stop)
        {
            if (group == null || subject == null || place == null || type == null || dayOfWeek == null || start == null || stop == null || LoginWrapper == null)
                return false;

            int SubjectID = subject.Id;
            int GroupID = group.GroupId;
            int PlaceID = place.Id;

            

            int rows = await LoginWrapper.DBHandler.ExecuteNonQueryAsync(
                "INSERT INTO zajecie (id, dzien_tygodnia, godzina_start, godzina_stop, id_przedmiotu, id_grupy, id_miejsca) VALUES " +
                "(NULL, @dayOfWeek, @startTime, @stopTime, @subjectID, @groupID, @placeID);",
                new()
                {
                    {"@dayOfWeek", dayOfWeek},
                    { "@starttime", start.Value.ToString("HH:mm")},
                    {"@stopTime", stop.Value.ToString("HH:mm") },
                    {"@subjectID", SubjectID },
                    {"@groupID", GroupID },
                    {"@placeID", PlaceID }
                });
            return rows == 1;
        }

      
    }


}
