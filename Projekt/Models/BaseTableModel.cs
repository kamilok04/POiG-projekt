using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class BaseTableModel 
    {
        private LoginWrapper? _loginWrapper;
        public LoginWrapper? LoginWrapper
        {
            get => _loginWrapper;
            init => _loginWrapper = value ?? throw new ArgumentNullException(nameof(value));
        }

        public BaseTableModel(LoginWrapper loginWrapper)
        {
            LoginWrapper = loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper));
        }
        public async Task<List<Dictionary<string, object>>> RetrieveDefaultQuery()
        {
            if (_loginWrapper == null)
            {
                throw new InvalidOperationException("LoginWrapper is not initialized.");
            }

            var defaultQuery = ((ITable)this).DefaultQuery;
            if (string.IsNullOrWhiteSpace(defaultQuery))
            {
                throw new InvalidOperationException("DefaultQuery cannot be null or empty.");
            }

            try
            {
                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync(defaultQuery);
                return result ?? new List<Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve default query.", ex);
            }
        }
    }
}
