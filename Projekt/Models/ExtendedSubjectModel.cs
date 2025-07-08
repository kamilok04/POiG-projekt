using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Projekt.Models
{
    public class ExtendedSubjectModel : SubjectModel
    {
        public string FacultyName { get; private set; }
        public string Description { get; private set; }
        public string Literature { get; private set; }
        public string PassConditions { get; private set; }

        public ExtendedSubjectModel(Dictionary<string, object> data) : base(data)
        {
            FacultyName = (string?)data["wydzial_org"] ?? "Czekaj...";
            Description = (string?)data["opis"] ?? "Czekaj...";
            Literature = (string?)data["literatura"] ?? "Czekaj...";
            PassConditions = (string?)data["warunki_zaliczenia"] ?? "Czekaj...";
        }

        public ExtendedSubjectModel(SubjectModel subject) : base(subject)
        {
            FacultyName = "Czekaj...";
            Description = "Czekaj...";
            Literature = "Czekaj...";
            PassConditions = "Czekaj...";

        }

        public ExtendedSubjectModel(SubjectModel baseSubject, Dictionary<string, object> extensionData)
            : base(baseSubject)
        {
            FacultyName = (string?)extensionData["nazwa"] ?? "Czekaj...";
            Description = (string?)extensionData["opis"] ?? "Czekaj...";
            Literature = (string?)extensionData["literatura"] ?? "Czekaj...";
            PassConditions = (string?)extensionData["warunki_zaliczenia"] ?? "Czekaj...";
        }

        public async Task GetDetails(LoginWrapper wrapper)
        {
            DatabaseHandler DBHandler = wrapper.DBHandler;
            if (DBHandler == null)
                throw new InvalidOperationException("DBHandler is not initialized in LoginWrapper.");
            var result = await DBHandler.ExecuteQueryAsync("SELECT o.opis, l.literatura, wz.warunki_zaliczenia, w.nazwa " +
                "FROM dane_przedmiotu dp, opis o, literatura l, warunki_zaliczenia wz, wydzial w " +
                "WHERE o.id = id_opisu " +
                "AND l.id = id_literatury " +
                "AND wz.id = id_warunkow " +
                "AND w.nazwa_krotka = wydzial_org " +
                "AND dp.id = @subjectDataID", new() { { "@subjectDataID", DataId } });
            if (result != null && result.Count > 0)
            {
                var extensionData = result[0];
                if (extensionData == null)
                    throw new InvalidOperationException("Nie udało się pobrać szczegółów przedmiotu.");
                FacultyName = (string)extensionData["nazwa"];
                Description = (string)extensionData["opis"];
                Literature = (string)extensionData["literatura"];
                PassConditions = (string)extensionData["warunki_zaliczenia"];
            }
            else
            {
                throw new InvalidOperationException("Nie udało się pobrać szczegółów przedmiotu.");
            }

        }
    }
}
