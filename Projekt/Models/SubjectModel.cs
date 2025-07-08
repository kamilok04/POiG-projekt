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

    public class SubjectModel
    {
        public int Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public int DescriptionId { get; init; }
        public int LiteratureId { get; init; }
        public int PassConditionsId { get; init; }
        public Int16 Credits { get; init; }
        public string FacultyId { get; init; }
        public int DataId { get; init; }

        // Constructor to initialize properties from a dictionary  
        public SubjectModel(Dictionary<string, object> data)
        {
            Id = (int)data["id"];
            DataId = (int)data["id_danych"];
            Code = (string)data["kod"];
            Name = (string)data["nazwa"];
            DescriptionId = (int)data["id_opisu"];
            LiteratureId = (int)data["id_literatury"];
            PassConditionsId = (int)data["id_warunkow"];
            Credits = (Int16)data["punkty"];
            FacultyId = (string)data["wydzial_org"];
        }

        // Copy constructor  
        public SubjectModel(SubjectModel model)
        {
            Id = model.Id;
            Code = model.Code;
            Name = model.Name;
            DescriptionId = model.DescriptionId;
            LiteratureId = model.LiteratureId;
            PassConditionsId = model.PassConditionsId;
            Credits = model.Credits;
            FacultyId = model.FacultyId;
            DataId = model.DataId;
        }

        public override string ToString()
        {
            return $"{FacultyId}>{Name} ({Code})";
        }
    }
}
