using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class LessonModel : ObservableObject, ITable
    {
        string ITable.TableName => "zajecie";
        string? ITable.DefaultQuery => "SELECT id, dzien_tygodnia, godzina_start, godzina_stop, id_przedmiotu, id_grupy, id_miejsca " +
                "FROM zajecie " +
                "WHERE id = @id";
        Dictionary<string, object>? ITable.DefaultParameters => new() { { "@id", _id } };

        private int _id { get; set; }
        private string _dayOfWeek { get; set; } = string.Empty; // Fix for CS8618
        private TimeOnly _timeStart { get; set; }
        private TimeOnly _timeEnd { get; set; }
        private SubjectModel? _subject { get; set; }
        private PlaceModel? _place { get; set; }
        private GroupEditModel? _group { get; set; }
        private List<string>? _coordinators { get; set; }

        private int PlaceID { get; set; }
        private int GroupID { get; set; }
        private int SubjectID { get; set; }

        

        public LessonModel(Dictionary<string, object> row)
        {
            Id = (int)row["id"];
            TimeStart = TimeOnly.FromTimeSpan((TimeSpan)row["godzina_start"]);
            TimeEnd = TimeOnly.FromTimeSpan((TimeSpan)row["godzina_stop"]);
            DayOfWeek = (string)row["dzien_tygodnia"];
            PlaceID = (int)row["id_miejsca"];
            SubjectID = (int)row["id_przedmiotu"];
            GroupID = (int)row["id_grupy"];
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

        public SubjectModel? Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        public PlaceModel? Place
        {
            get { return _place; }
            set
            {
                _place = value;
                OnPropertyChanged(nameof(Place));
            }
        }

        public GroupEditModel? Group
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

        public string DayOfWeek
        {
            get { return _dayOfWeek; }
            set
            {
                _dayOfWeek = value;
                OnPropertyChanged(nameof(DayOfWeek));
            }
        }

        public async Task LoadDataAsync(LoginWrapper wrapper)
        {
            Place = await RetrieveService.GetAsync<PlaceModel>(wrapper, PlaceID);
            Subject = await RetrieveService.GetAsync<SubjectModel>(wrapper, SubjectID);
            Group = new(wrapper);
            await Group.LoadGroupData(GroupID);
        }
    }


}
