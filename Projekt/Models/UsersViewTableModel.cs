using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    class UsersViewTableModel : ITable
    {
        string ITable.TableName => "users_view";
         string ITable.DefaultQuery => "SELECT * FROM users_view";
        Dictionary<string, object>? ITable.DefaultParameters => null;
        private LoginWrapper _loginWrapper;

        public UsersViewTableModel(LoginWrapper loginWrapper)
        {
            _loginWrapper = loginWrapper;
            ;
        }

        public async Task<List<Dictionary<string, object>>> RetrieveDefaultQuery()
        {
            List<Dictionary<string, object>> result = [];
            await _loginWrapper.DBHandler.ExecuteQueryAsync(((ITable)this).DefaultQuery)
                .ContinueWith(task =>
                {
                    if(task.IsCompletedSuccessfully && task.Result.Count > 0)
                    {
                        result = task.Result;
                    }
                    
                });

            return result;
        }
    }
}
