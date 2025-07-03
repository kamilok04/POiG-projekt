using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class LessonModel : BaseTableModel, ITable
    {

        string ITable.TableName => "zajecie";
        string? ITable.DefaultQuery => "SELECT dzien_tygodnia, godzina_start, godzina_stop, id_przedmiotu, id_grupy, id_miejsca " +
                "FROM zajecie " +
                "WHERE id = @id";
        Dictionary<string, object>? ITable.DefaultParameters => new() { { "@id", _id } };

        private int _id { get; set; }
        private TimeOnly _timeStart { get; set; }
        private TimeOnly _timeEnd { get; set; }
        private SubjectModel? _subject { get; set; }
        private PlaceModel? _place { get; set; }
        private GroupEditModel? _group { get; set; }
        private List<string>? _coordinators { get; set; }

        public LessonModel(int id, LoginWrapper wrapper)  : base(wrapper)
        {
            _id = id;
    
           
        }


        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public TimeOnly TimeStart
        {
            get { return _timeStart; }
            set
            {
                _timeStart = value;
                OnPropertyChanged(nameof(TimeStart));
            }
        }

        public TimeOnly TimeEnd
        {
            get { return _timeEnd; }
            set
            {
                _timeEnd = value;
                OnPropertyChanged(nameof(TimeEnd));
            }
        }

        public SubjectModel Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        public PlaceModel Place
        {
            get { return _place; }
            set
            {
                _place = value;
                OnPropertyChanged(nameof(Place));
            }
        }

        public GroupEditModel Group
        {
            get => _group;
            set
            {
                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        public List<string> Coordinators
        {
            get { return _coordinators; }
            set
            {
                _coordinators = value;
                OnPropertyChanged(nameof(Coordinators));
            }
        }

 

        public async Task InitializeAsync()
        {
            var DBHandler = LoginWrapper.DBHandler;
            var result = await DBHandler.ExecuteQueryAsync(
                "SELECT dzien_tygodnia, godzina_start, godzina_stop, id_przedmiotu, id_grupy, id_miejsca " +
                "FROM zajecie " +
                "WHERE id = @id", new() { { "@id", _id } });
            if (result == null) return;
            var row = result[0];

            TimeStart = (TimeOnly)row["godzina_start"];
            TimeEnd = (TimeOnly)row["godzina_stop"];

            int PlaceID = (int)row["id_miejsca"];
            int SubjectID = (int)row["id_przedmiotu"];
            int GroupID = (int)row["id_grupy"];

            Place = new(DBHandler.ExecuteQueryAsync(
                "SELECT * FROM miejsce WHERE id = @id", new() { { "@id",PlaceID} }).Result[0]);
            Subject = new(DBHandler.ExecuteQueryAsync(
                "SELECT * from przedmiot WHERE id = @id;", new() { { "@id", SubjectID } }).Result[0]);
            Group = new(LoginWrapper);
            await Group.LoadGroupData(GroupID);




        }

    }


}
