using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Miscellaneous
{
    public static class RetrieveService
    {

        public static async Task<T?> GetAsync<T>(LoginWrapper wrapper, string login) where T : class
        {
            if (typeof(T) == typeof(CoordinatorModel)) 
                return await GetCoordinatorAsync(wrapper, login) as T;
            else return null;

        }
        public static async Task<List<T>?> GetAllAsync<T>(LoginWrapper wrapper) where T : class
        {
            if (typeof(T) == typeof(SubjectModel))
            {
                var subjects = await GetAllSubjectsAsync(wrapper);
                return subjects as List<T>;
            }
            else if (typeof(T) == typeof(PlaceModel))
            {
                var places = await GetAllPlacesAsync(wrapper);
                return places as List<T>;
            }
            else if (typeof(T) == typeof(GroupEditModel))
            {
                var places = await GetAllGroupsAsync(wrapper);
                return places as List<T>;
            }
            else if (typeof(T) == typeof(CoordinatorModel))
            {
                var coordinators = await GetAllCoordinatorsAsync(wrapper);
                return coordinators as List<T>;
            }
            else if (typeof(T) == typeof(StudentModel))
            {
                var students = await GetAllStudentsAsync(wrapper);
                return students as List<T>;
            }
            else if (typeof(T) == typeof(UserModel))
            {
                var teachers = await GetAllUsersAsync(wrapper);
                return teachers as List<T>;
            }

            return null;
        }

        private static async Task<CoordinatorModel?> GetCoordinatorAsync(LoginWrapper loginWrapper, string login)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                           "SELECT * " +
                           "FROM dane_uzytkownika " +
                           "WHERE login = (SELECT login FROM prowadzacy WHERE login = @login);", new() { { "@login", login} });
            return new CoordinatorModel(result[0]);
        }
        private static async Task<List<UserModel>?> GetAllUsersAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * " +
                "FROM dane_uzytkownika;");
            return result.Select(row => new UserModel(row)).ToList();
        }
        private static async Task<List<StudentModel>?> GetAllStudentsAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * " +
                "FROM dane_uzytkownika " +
                "WHERE login IN (SELECT login FROM student);");
            return result.Select(row => new StudentModel(row)).ToList();
        }
        private static async Task<List<CoordinatorModel>?> GetAllCoordinatorsAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * " +
                "FROM dane_uzytkownika " +
                "WHERE login IN (SELECT login FROM prowadzacy);");
            return result.Select(row => new CoordinatorModel(row)).ToList();
        }

        private static async Task<List<SubjectModel>?> GetAllSubjectsAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT przedmiot.id, id_danych, kod, nazwa, id_opisu, id_literatury, id_warunkow, punkty, wydzial_org " +
                "FROM przedmiot " +
                "JOIN dane_przedmiotu " +
                "ON dane_przedmiotu.id = przedmiot.id_danych");
            var subjects = new List<SubjectModel>();
            foreach (var row in result)
            {
                var subject = new SubjectModel(row);
                subjects.Add(subject);
            }
            return subjects;
        }

        private static async Task<List<PlaceModel>?> GetAllPlacesAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
              "SELECT id, id_wydzialu, id_adresu, numer, pojemnosc " +
              "FROM miejsce;");
            return result.Select(row => new PlaceModel(row)).ToList();
        }

        private static async Task<List<GroupEditModel>?> GetAllGroupsAsync(LoginWrapper loginWrapper)
        {
            var model = new GroupEditModel(loginWrapper);
            return await model.GetAllGroupsAsync();
        }

    }
}
