using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    class MajorManagementModel(LoginWrapper wrapper)
    {
        public async Task<int> HandleSave(int majorID, int dataID, string name, string facultyId)
        {
            if (majorID <= 0 || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(facultyId))
            {
                return 0;
            }
           
            // nazwa
            int code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE dane_kierunku SET nazwa = @name WHERE id = @dataId", new() { { "@name", name }, { "@dataId", dataID } });
            if (code != 1) return 0;

            // kierunek
            var data = new Dictionary<string, object>
            {
                { "@id", majorID },
                { "@facultyId", facultyId }
            };

            code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE kierunek SET id_wydzialu = @facultyId WHERE id = @id", data);
            return code;
        }

        public async Task<int> HandleDelete(int majorID)
        {
            if (majorID <= 0)
            {
                return 0;
            }
            // Usuwanie kierunku
            int code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "DELETE FROM kierunek WHERE id = @id", new() { { "@id", majorID } });

            return code;
        }
    }
}
