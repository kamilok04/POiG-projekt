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
    public class GroupViewTableModel (LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => String.Empty;
        public string DefaultQuery => """
            SELECT 
                g.id "ID grupy", g.numer "Numer Grupy", w.nazwa_krotka "Wydział", dk.nazwa "Kierunek", r.semestr "Semestr"
            FROM 
                grupa g 
            JOIN 
                rocznik r 
            ON 
                r.id = g.id_rocznika
            JOIN 
                kierunek k
            ON 
                k.id = r.id_kierunku
            JOIN 
                wydzial w
            ON 
                w.nazwa_krotka = k.id_wydzialu
            JOIN 
                dane_kierunku dk
            ON 
                dk.id = k.id_danych_kierunku;
            """;
        Dictionary<string, object>? ITable.DefaultParameters => null;
    }
}
