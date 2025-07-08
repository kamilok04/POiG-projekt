using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    internal class SubjectEditModel(LoginWrapper wrapper)
    {

        public async Task<bool> HandleUpdate(
            int SubjectDataID,
            int LiteratureId,
            int DescriptionId,
            int PassConditionsId,
            string FacultyId,
            string Name,
            string Code,
            Int16 Points,
            string? Literature,
            string? Description,
            string? PassConditions

  )
        {
            // description
            int code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE opis SET opis = @desc WHERE id = @id",
                new() { { "@desc", Description ?? "" }, { "@id", DescriptionId } });
            if (code != 1) return false;

            // pass conditions
            code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE warunki_zaliczenia SET warunki_zaliczenia = @pass WHERE id = @id",
                new() { { "@pass", PassConditions ?? "" }, { "@id", PassConditionsId } });

            if (code != 1) return false;
            // literature
            code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE literatura SET literatura = @literature WHERE id = @id",
                new() { { "@literature", Literature ?? "" }, { "@id", LiteratureId } });

            if (code != 1) return false;

            // subject
            code = await wrapper.DBHandler.ExecuteNonQueryAsync(
                "UPDATE dane_przedmiotu SET kod = @code, nazwa = @name, punkty = @points, wydzial_org = @facultyId WHERE id = @id",
                new()
                {
                    { "@code", Code },
                    { "@name", Name },
                    { "@points", Points },
                    { "@facultyId", FacultyId },
                    { "@id", SubjectDataID }
                });

            if (code != 1) return false;
            return true;
        }

        public async Task<bool> HandleDelete(int id)
        {
            int code = await wrapper.DBHandler.ExecuteNonQueryAsync("DELETE FROM przedmiot WHERE id = @id", new() { { "@id", id } });
            return code == 1;
        }
    }
}
