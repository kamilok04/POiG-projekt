using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class SubjectViewTableModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => String.Empty;
        public string? DefaultQuery => """
            SELECT 
                dp.kod,
                dp.nazwa,
                o.opis,
                l.literatura,
                wz.warunki_zaliczenia,
                dp.punkty,
                w.nazwa AS wydział
            FROM 
                dane_przedmiotu dp
            JOIN 
                opis o ON dp.id_opisu = o.id
            JOIN 
                literatura l ON dp.id_literatury = l.id
            JOIN 
                warunki_zaliczenia wz ON dp.id_warunkow = wz.id
            JOIN 
                wydzial w ON dp.wydzial_org = w.nazwa_krotka;
            """;
        Dictionary<string, object>? ITable.DefaultParameters => null;

    }
}
