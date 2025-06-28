using Org.BouncyCastle.Asn1.Mozilla;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Projekt.Models
{
    public class UsersEditModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        string ITable.TableName => "users_view";
        string ITable.DefaultQuery => "UPDATE uzytkownik set @col = @value WHERE login = @login";
        Dictionary<string, object>? ITable.DefaultParameters => new(){
            {"@col", Column },
            {"@value", Value },
            { "@login", Login} };

        private string _column;
        public string Column
        {
            get => _column;
            set
            {
                _column = value;
                
            }
        }
        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                _value = value;

            }
        }
        public string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;

            }
        }

        public async Task<bool> UpdateDataItem(string column, object value, string login)
        {
            return 1 == await LoginWrapper.DBHandler.ExecuteNonQueryAsync(((ITable)this).DefaultQuery, ((ITable)this).DefaultParameters);
        }

       
    }
}
