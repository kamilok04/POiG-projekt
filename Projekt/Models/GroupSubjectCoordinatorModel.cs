using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class GroupSubjectCoordinatorModel(LoginWrapper wrapper) : BaseTableModel(wrapper), ITable
    {
        string ITable.TableName => "grupa_przedmiot_prowadzacy";
        string? ITable.DefaultQuery =>
            "SELECT id_prowadzacego " +
            "FROM grupa_przedmiot_prowadzacy " +
            "WHERE id_grupy = @groupID " +
            "AND id_przedmiotu = @subjectID; ";

        private List<MySqlCommand> assignments = new();
        Dictionary<string, object>? ITable.DefaultParameters => null;

        public GroupEditModel? Group { get; set; }
        public SubjectModel? Subject { get; set; }

        public int? GroupID
        {
            get => Group?.GroupId;

        }

        public int? SubjectID
        {
            get => Subject?.Id;
        }

        public void AssignCoordinator(CoordinatorModel coordinator)
        {
            if (coordinator._login == null || GroupID == null || SubjectID == null)
                return;
            var command = DatabaseHandler.CreateCommand(
                $"INSERT INTO {((ITable)this).TableName} " +
                "(id_grupy, id_prowadzacego, id_przedmiotu) " +
               "VALUES (@groupID, @coordinatorID, @subjectID);", new()
                {
                    {"@groupID", GroupID },
                    {"@coordinatorID", coordinator._login },
                    {"@subjectID", SubjectID }
                });
            assignments.Add(command);

        }


        public async Task<List<Dictionary<string, object>>?> GetAssignedCoordinators()
        {
            if (LoginWrapper == null || GroupID == null || SubjectID == null) return null;
            return await LoginWrapper.DBHandler.ExecuteQueryAsync(
                 "SELECT id_prowadzacego " +
            "FROM grupa_przedmiot_prowadzacy " +
            "WHERE id_grupy = @groupID " +
            "AND id_przedmiotu = @subjectID; ", new()
        {
            {"@groupID", GroupID },
            {"@subjectID", SubjectID }
        }
                 );
        }
        public async Task<bool> ExecuteAssignments()
        {
            if (assignments == null || LoginWrapper == null) return false;
            MySqlCommand[] commands =
                [
                    DatabaseHandler.CreateCommand(
                        $"DELETE FROM {((ITable)this).TableName} " +
                        "WHERE id_grupy = @groupID " +
                        "AND id_przedmiotu = @subjectID;",
                        new()
        {
            {"@groupID", GroupID },
            {"@subjectID", SubjectID }
        }),
                     .. assignments
                ];

            assignments.Clear();
            return await LoginWrapper.DBHandler.ExecuteInTransactionAsync(commands);

        }






    }
}
