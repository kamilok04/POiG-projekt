using MySql.Data.Types;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class LessonsEditModel(LoginWrapper wrapper) : ObservableObject
    {

        public async Task<bool> UpdateAsync(string dayOfWeek, TimeOnly? timeStart, TimeOnly? timeEnd,
            int placeID, int subjectID, int groupID, int ID)
        {
            if(timeStart == null ||  timeEnd == null) return false;
            int result = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE zajecie SET dzien_tygodnia = @dayOfWeek, godzina_start = @timeStart, " +
                "godzina_stop = @timeStop, id_miejsca = @placeID, id_przedmiotu = @subjectID, id_grupy = @groupID " +
                "WHERE id = @id;", new()
                {
                    ["@dayOfWeek"] = dayOfWeek.ToLower(),
                    ["@timeStart"] = timeStart.Value.ToString("HH:mm"),
                    ["@timeStop"] = timeEnd.Value.ToString("HH:mm"),
                    ["@placeID"] = placeID,
                    ["@subjectID"] = subjectID,
                    ["@groupID"] = groupID,
                    ["@id"] = ID
                }
            );
            return result == 1;
        }

    }
}
