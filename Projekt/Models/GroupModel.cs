using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class GroupModel
    {
        public int Id { get; init; }
        public string? GroupNumber { get; init; }
        public string? Faculty { get; init; }
        public string? Degree { get; init; }
        public string? Semester { get; init; }
    }
}
