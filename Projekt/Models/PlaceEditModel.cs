using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class PlaceEditModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public int PlaceId { get; set; }
        public string? Faculty { get; set; }
        public int ClassNumber { get; set; }
        public string? Address { get; set; }
        public string? Capacity { get; set; }

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "miejsce";

        public string? DefaultQuery => null; // Nie używamy domyślnego zapytania dla edycji

        public Dictionary<string, object>? DefaultParameters => null;

        public async Task<bool> UpdatePlace()
        {
            try
            {
                var facultyIdQuery = @"
                    SELECT nazwa_krotka 
                    FROM wydzial 
                    WHERE nazwa = @facultyName";

                var facultyParams = new Dictionary<string, object>
                {
                    { "@facultyName", Faculty ?? string.Empty }
                };

                var facultyResult = await DatabaseHandler.ExecuteQueryAsync(facultyIdQuery, facultyParams);
                if (facultyResult == null || !facultyResult.Any())
                {
                    return false;
                }

                var facultyId = facultyResult.First()["nazwa_krotka"]?.ToString();



                var addressIdQuery = @"
                    SELECT id_adresu 
                    FROM miejsce 
                    WHERE id = @placeId";

                var addressIdParams = new Dictionary<string, object>
                {
                    { "@placeId", PlaceId }
                };

                var addressIdResult = await DatabaseHandler.ExecuteQueryAsync(addressIdQuery, addressIdParams);
                if (addressIdResult == null || !addressIdResult.Any())
                {
                    return false;
                }

                var addressId = Convert.ToInt32(addressIdResult.First()["id_adresu"]);



                var updateAddressQuery = @"
                    UPDATE adres 
                    SET adres = @address 
                    WHERE id = @addressId";

                var updateAddressParams = new Dictionary<string, object>
                {
                    { "@addressId", addressId },
                    { "@address", Address ?? string.Empty }
                };

                var updateAddressCommand = DatabaseHandler.CreateCommand(updateAddressQuery, updateAddressParams);



                var updatePlaceQuery = @"
                    UPDATE miejsce 
                    SET id_wydzialu = @facultyId, 
                        numer = @classNumber, 
                        pojemnosc = @capacity 
                    WHERE id = @placeId";

                var updatePlaceParams = new Dictionary<string, object>
                {
                    { "@placeId", PlaceId },
                    { "@facultyId", facultyId ?? string.Empty },
                    { "@classNumber", ClassNumber },
                    { "@capacity", int.TryParse(Capacity, out int cap) ? cap : 0 }
                };

                var updatePlaceCommand = DatabaseHandler.CreateCommand(updatePlaceQuery, updatePlaceParams);


                return await DatabaseHandler.ExecuteInTransactionAsync(updateAddressCommand, updatePlaceCommand);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}