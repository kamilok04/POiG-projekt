using Org.BouncyCastle.Asn1.Mozilla;
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
    public class PlaceViewTableModel (LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        string ITable.TableName => "miejsce";
        string ITable.DefaultQuery => "SELECT * FROM miejsce";
        Dictionary<string, object>? ITable.DefaultParameters => null;
    }
}
