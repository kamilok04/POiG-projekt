using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt.Models
{
    public class PlaceModel
    {
        public int Id { get; init; }
        public string FacultyId {  get; init; }
        public int AddressId {  get; init; }
        public string Number { get; init; }
        public int Capacity {  get; init; }
        public string? Address { get; set; }



        public PlaceModel(Dictionary<string, object> data)
        {
            Id = (int)data["id"];
            FacultyId = (string)data["id_wydzialu"];
            AddressId = (int)data["id_adresu"];
            Number = (string)data["numer"];
            Capacity = (int)data["pojemnosc"];
        }

      

        public override string ToString()
        {
            return $"{FacultyId}-{Number} ({Capacity} miejsc)";
        }

        public string? GetAddress(LoginWrapper loginWrapper)
        {
           var result = loginWrapper.DBHandler.ExecuteQueryAsync("SELECT adres FROM adres WHERE id = @id;",
               new() { { "@id", AddressId } }).Result;
      
            Address = result[0]["adres"] as string;
            return Address;
        }
    }
}
