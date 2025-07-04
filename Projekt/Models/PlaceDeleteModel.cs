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
        public string? Faculty { get; set; }
        public int ClassNumber { get; set; }
        public string? Address { get; set; }
        public string? Capacity { get; set; }

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "miejsce";

        public string? DefaultQuery => null; // Nie używamy domyślnego zapytania dla edycji

        public Dictionary<string, object>? DefaultParameters => null;

        public async Task<bool> DeletePlace()
        {
            try
            {
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

                var deletePlaceQuery = @"
                    DELETE FROM miejsce 
                    WHERE id = @placeId";

                var deletePlaceParams = new Dictionary<string, object>
                {
                    { "@placeId", PlaceId }
                };

                var deletePlaceCommand = DatabaseHandler.CreateCommand(deletePlaceQuery, deletePlaceParams);

                // Usuń adres (jeśli nie jest używany przez inne miejsca)
                var deleteAddressQuery = @"
                    DELETE FROM adres 
                    WHERE id = @addressId 
                    AND NOT EXISTS (
                        SELECT 1 FROM miejsce 
                        WHERE id_adresu = @addressId 
                        AND id != @placeId
                    )";

                var deleteAddressParams = new Dictionary<string, object>
                {
                    { "@addressId", addressId },
                    { "@placeId", PlaceId }
                };

                var deleteAddressCommand = DatabaseHandler.CreateCommand(deleteAddressQuery, deleteAddressParams);

                // Wykonaj oba zapytania w transakcji
                return await DatabaseHandler.ExecuteInTransactionAsync(deletePlaceCommand, deleteAddressCommand);
            }
            catch (Exception ex)
            {
                return false;
                // throw new Exception($"Usunięcie miejsca nie powiodło się: {ex.Message}");
            }
        }
    }
}
