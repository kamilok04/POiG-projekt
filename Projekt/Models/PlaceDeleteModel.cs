using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class PlaceDeleteModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public int PlaceId { get; set; }
        public string? BuildingCode { get; set; }
        public List<string> Faculties = new List<string> { "MS", "MT", "AEI", "Ch" };
        public int ClassNumber { get; set; }
        public string? Address { get; set; }
        public string? Capacity { get; set; }
        public string? CurrentFaculty { get; set; }

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "Places";
        // No default query for deletion
        public string DefaultQuery => "DELETE FROM Places WHERE id = @placeId";
        public Dictionary<string, object>? DefaultParameters => new() {
            { "@placeId",PlaceId}
        };
        // Additional methods for deletion can be added here if needed

        public PlaceDeleteModel(LoginWrapper loginWrapper, int placeId) : this(loginWrapper)
        {
        }

        public async Task<bool> DeletePlace()
        {
            try
            {
                var getIDQuery = "SELECT id FROM Places WHERE id = @placeId";
                var getIDParams = new Dictionary<string, object>
                {
                    { "@placeId", PlaceId }
                };

                var addressIdResult = await DatabaseHandler.ExecuteQueryAsync(getIDQuery, getIDParams);
                int? addressId = null;

                if (addressIdResult != null && addressIdResult.Count > 0 && addressIdResult[0].ContainsKey("id"))
                {
                    addressId = Convert.ToInt32(addressIdResult[0]["id"]);
                }

                var deletePlaceCommand = DatabaseHandler.CreateCommand(DefaultQuery, DefaultParameters);

                MySqlCommand? deleteAddressCommand = null;
                if (addressId.HasValue)
                {
                    var deleteAddressQuery = "DELETE FROM Adress WHERE id = @addressId";
                    var deleteAddressParams = new Dictionary<string, object>
                    {
                        { "@addressId", addressId.Value }
                    };
                    deleteAddressCommand = DatabaseHandler.CreateCommand(deleteAddressQuery, deleteAddressParams);
                }

                if (deleteAddressCommand != null)
                {
                    return await DatabaseHandler.ExecuteInTransactionAsync(deletePlaceCommand, deleteAddressCommand);
                }
                else
                {
                    return await DatabaseHandler.ExecuteInTransactionAsync(deletePlaceCommand);
                }
            }
            catch (Exception)
            {
                throw new Exception("Query failed to execute. Please check the parameters and connection.");
            }
        }

        public async Task<Dictionary<string, object>> GetPlaceById(int placeId)
        {
            try
            {
                var query = @"
                    SELECT m.id, m.id_wydzialu, m.numer, m.pojemnosc, a.adres, w.nazwa_krotka as buildingCode
                    FROM miejsce m
                    JOIN adres a ON m.id_adresu = a.id
                    JOIN wydzial w ON m.id_wydzialu = w.nazwa_krotka
                    WHERE m.id = @placeId";

                var parameters = new Dictionary<string, object>
                {
                    { "@placeId", placeId }
                };

                var results = await DatabaseHandler.ExecuteQueryAsync(query, parameters);
                return results?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve place by ID.", ex);
            }
        }
    }
}
