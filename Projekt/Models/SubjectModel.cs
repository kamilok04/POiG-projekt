using Org.BouncyCastle.Asn1.Cmp;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public readonly struct SubjectData
    {
        public int Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public int? DescriptionId { get; init; }
        public int? LiteratureId { get; init; }
        public int? PassConditionsId { get; init; }
        public int Credits { get; init; }
        public string FacultyId { get; init; }

        public SubjectData(Dictionary<string, object> attributes)
        {
            Id = (int)attributes["id"];
            Code = (string)attributes["kod"];
            Name = (string)attributes["nazwa"];
            DescriptionId = (int)attributes["id_opisu"];
            LiteratureId = (int)attributes["id_literatury"];
            PassConditionsId = (int)attributes["id_warunkow"];
            Credits = (int)attributes["punkty"];
            FacultyId = (string)attributes["wydzial_org"];


        }
    }

    public class SubjectModel
    {
        public int Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public int? DescriptionId { get; init; }
        public int? LiteratureId { get; init; }
        public int? PassConditionsId { get; init; }
        public Int16 Credits { get; init; }
        public string FacultyId { get; init; }
        public int? DataId { get; init; }

        public SubjectData? SubjectData { get; set; }

   
        public SubjectModel(Dictionary<string, object> data)
        {
            Id = (int)data["id"];
            DataId = (int?)data["id_danych"];
      
            Code = (string)data["kod"];
            Name = (string)data["nazwa"];
            DescriptionId = (int)data["id_opisu"];
            LiteratureId = (int)data["id_literatury"];
            PassConditionsId = (int)data["id_warunkow"];
            Credits = (Int16)data["punkty"];
            FacultyId = (string)data["wydzial_org"];
        }
        



        public override string ToString()
        {
            return $"{FacultyId}>{Name} ({Code})";

        }

    }
}
