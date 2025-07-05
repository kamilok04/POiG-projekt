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
