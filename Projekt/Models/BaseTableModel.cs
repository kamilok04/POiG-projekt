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
        private LoginWrapper _loginWrapper;
        public LoginWrapper LoginWrapper
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
            List<Dictionary<string, object>> result = [];
            await _loginWrapper.DBHandler.ExecuteQueryAsync(((ITable)this).DefaultQuery)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully && task.Result.Count > 0)
                    {
                        result = task.Result;
                    }

                });

            return result;
        }
    }
}
