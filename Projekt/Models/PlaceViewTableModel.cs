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
        public string TableName => String.Empty;
        public string DefaultQuery => """
            SELECT 
                p.id AS "ID miejsca",
                w.nazwa AS "Wydział",
                a.adres AS "Numer pokoju",
                p.numer AS "Numer sali",
                p.pojemnosc AS "Pojemność"
            FROM 
                miejsce p
            JOIN wydzial w 
                ON p.id_wydzialu = w.nazwa_krotka
            JOIN adres a
                ON p.id_adresu = a.id;
            """;
        Dictionary<string, object>? ITable.DefaultParameters => null;
    }
}
