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
        public string? BuildingCode { get; set; }
        public List<string> Faculties = new List<string> { "MS", "MT", "AEI", "Ch" };
        public int ClassNumber { get; set; }
        public string? Address { get; set; }
        public string? Capacity { get; set; }
        public string? CurrentFaculty { get; set; }

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "Places";

        public string? DefaultQuery => BuildDefaultQuery();

        public Dictionary<string, object>? DefaultParameters => new()
        {
            { "@placeId", PlaceId },
            { "@buildingCode", BuildingCode ?? string.Empty },
            { "@classNumber", ClassNumber },
            { "@address", Address ?? string.Empty },
            { "@capacity", Capacity ?? string.Empty },
            { "@currentFaculty", CurrentFaculty ?? string.Empty }
        };
        // Additional methods for deletion can be added here if needed

        public async Task<bool> UpdatePlace()
        {
            try
            {
                // Najpierw aktualizuj adres
                var updateAddressQuery = @"
                    UPDATE adres a 
                    JOIN miejsce m ON a.id = m.id_adresu 
                    SET a.adres = @address 
                    WHERE m.id = @placeId";

                var updateAddressParams = new Dictionary<string, object>
                {
                    { "@placeId", PlaceId },
                    { "@address", Address ?? string.Empty }
                };

                var updateAddressCommand = DatabaseHandler.CreateCommand(updateAddressQuery, updateAddressParams);

                // Następnie aktualizuj miejsce
                var updatePlaceQuery = @"
                    UPDATE miejsce 
                    SET id_wydzialu = @currentFaculty, 
                        numer = @classNumber, 
                        pojemnosc = @capacity 
                    WHERE id = @placeId";

                var updatePlaceParams = new Dictionary<string, object>
                {
                    { "@placeId", PlaceId },
                    { "@currentFaculty", CurrentFaculty ?? string.Empty },
                    { "@classNumber", ClassNumber },
                    { "@capacity", int.TryParse(Capacity, out int cap) ? cap : 0 }
                };

                var updatePlaceCommand = DatabaseHandler.CreateCommand(updatePlaceQuery, updatePlaceParams);

                // 2 zapytania w jednym poleceniu
                return await DatabaseHandler.ExecuteInTransactionAsync(updateAddressCommand, updatePlaceCommand);
            }
            catch (Exception)
            {
                throw new Exception("Query failed to execute. Please check the parameters and connection.");
            }
        }

        public async Task<Dictionary<string, object>?> GetPlaceById(int placeId)
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
            catch (Exception)
            {
                return null;
            }
        }

        public string BuildDefaultQuery()
        {
            if (PlaceId <= 0)
            {
                throw new InvalidOperationException("PlaceId must be greater than zero.");
            }
            return "UPDATE Places SET " +
                   "building_code = @buildingCode, " +
                   "class_number = @classNumber, " +
                   "address = @address, " +
                   "capacity = @capacity, " +
                   "current_faculty = @currentFaculty " +
                   "WHERE id = @placeId;";
        }
    }

}
