﻿using Org.BouncyCastle.Asn1.X509.Qualified;
using Projekt.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Projekt.Miscellaneous
{
    public static class RetrieveService
    {

        public static async Task<T?> GetAsync<T>(LoginWrapper wrapper, object key) where T : class
        {
            return typeof(T) switch
            {
                var t when t == typeof(CoordinatorModel) => await GetCoordinatorAsync(wrapper, (string)key) as T,
                var t when t == typeof(PlaceModel) => await GetPlaceAsync(wrapper, (int)key) as T,
                var t when t == typeof(UserModel) => await GetUserAsync(wrapper, (string)key) as T,
                var t when t == typeof(SubjectModel) => await GetSubjectAsync(wrapper, (int)key) as T,
                var t when t == typeof(GroupEditModel) => await GetGroupAsync(wrapper, (int)key) as T,
                var t when t == typeof(StudentModel) => await GetStudentAsync(wrapper, (string)key) as T,
                var t when t == typeof(FacultyModel) => await GetFacultyAsync(wrapper, (string)key) as T,
                var t when t == typeof(MajorDataModel) => await GetMajorDataAsync(wrapper, (int)key) as T,
                var t when t == typeof(MajorModel) => await GetMajorAsync(wrapper, (int)key) as T,
                var t when t == typeof(ExtendedStudentModel) => await GetExtendedStudentAsync(wrapper, (int)key) as T,
                _ => null
            };
        }
        public static async Task<List<T>?> GetAllAsync<T>(LoginWrapper wrapper) where T : class
        {
            return typeof(T) switch
            {
                var t when t == typeof(SubjectModel) => await GetAllSubjectsAsync(wrapper) as List<T>,
                var t when t == typeof(PlaceModel) => await GetAllPlacesAsync(wrapper) as List<T>,
                var t when t == typeof(GroupEditModel) => await GetAllGroupsAsync(wrapper) as List<T>,
                var t when t == typeof(CoordinatorModel) => await GetAllCoordinatorsAsync(wrapper) as List<T>,
                var t when t == typeof(StudentModel) => await GetAllStudentsAsync(wrapper) as List<T>,
                var t when t == typeof(UserModel) => await GetAllUsersAsync(wrapper) as List<T>,
                var t when t == typeof(LessonModel) => await GetAllLessonsAsync(wrapper) as List<T>,
                var t when t == typeof(FacultyModel) => await GetAllFacultiesAsync(wrapper) as List<T>,
                var t when t == typeof(MajorModel) => await GetAllMajorsAsync(wrapper) as List<T>,
                _ => null
            };
        }

        private static async Task<CoordinatorModel?> GetCoordinatorAsync(LoginWrapper loginWrapper, string login)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                           "SELECT * " +
                           "FROM dane_uzytkownika " +
                           "WHERE login = (SELECT login FROM prowadzacy WHERE login = @login);", new() { { "@login", login } });
            return new CoordinatorModel(result[0]);
        }
        private static async Task<PlaceModel?> GetPlaceAsync(LoginWrapper loginWrapper, int ID)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                          "SELECT id, id_wydzialu, id_adresu, numer, pojemnosc " +
                          "FROM miejsce " +
                          "WHERE id = @id;", new() { { "@id", ID } });
            return new PlaceModel(result[0]);
        }
        private static async Task<UserModel?> GetUserAsync(LoginWrapper loginWrapper, string login)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * " +
                "FROM dane_uzytkownika " +
                "WHERE login = @login;", new() { { "@login", login } });
            return new UserModel(result[0]);
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

        private static async Task<List<LessonModel>?> GetAllLessonsAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * FROM zajecie");
            return result.Select(row => new LessonModel(row)).ToList();
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
        private static async Task<SubjectModel?> GetSubjectAsync(LoginWrapper loginWrapper, int id)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT przedmiot.id, id_danych, kod, nazwa, id_opisu, id_literatury, id_warunkow, punkty, wydzial_org " +
                "FROM przedmiot " +
                "JOIN dane_przedmiotu ON dane_przedmiotu.id = przedmiot.id_danych " +
                "WHERE przedmiot.id = @id;", new() { { "@id", id } });
            if (result.Count == 0) return null;
            return new SubjectModel(result[0]);
        }

        private static async Task<GroupEditModel?> GetGroupAsync(LoginWrapper loginWrapper, int groupId)
        {
            var model = new GroupEditModel(loginWrapper);
            var loaded = await model.LoadGroupData(groupId);
            if (!loaded) return null;
            return model;
        }

        private static async Task<StudentModel?> GetStudentAsync(LoginWrapper loginWrapper, string login)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * " +
                "FROM dane_uzytkownika " +
                "WHERE login = (SELECT login FROM student WHERE login = @login);", new() { { "@login", login } });
            if (result.Count == 0) return null;
            return new StudentModel(result[0]);
        }

        private static async Task<FacultyModel?> GetFacultyAsync(LoginWrapper loginWrapper, string facultyId)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * FROM wydzial WHERE nazwa_krotka = @id;", new() { { "@id", facultyId } });
            if (result.Count == 0) return null;
            return new FacultyModel(result[0]);
        }

        private static async Task<MajorDataModel?> GetMajorDataAsync(LoginWrapper loginWrapper, int majorID)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT nazwa FROM dane_kierunku WHERE id = @id;", new() { { "@id", majorID } });
            if (result.Count == 0) return null;
            return new MajorDataModel((string)result[0]["nazwa"]);
        }

        private static async Task<List<FacultyModel>?> GetAllFacultiesAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * FROM wydzial;");
            return result.Select(row => new FacultyModel(row)).ToList();
        }
        private static async Task<MajorModel?> GetMajorAsync(LoginWrapper loginWrapper, int majorId)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * FROM kierunek WHERE id = @id;", new() { { "@id", majorId } });
            if (result.Count == 0) return null;
            return new MajorModel(result[0], loginWrapper);
        }

        private static async Task<List<MajorModel>?> GetAllMajorsAsync(LoginWrapper loginWrapper)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * FROM kierunek;");
            return result.Select(row => new MajorModel(row, loginWrapper)).ToList();
        }

        private static async Task<ExtendedStudentModel?> GetExtendedStudentAsync(LoginWrapper loginWrapper, int studentId)
        {
            var result = await loginWrapper.DBHandler.ExecuteQueryAsync(
                "SELECT * FROM dane_studenta " +
                "WHERE \"Nr indeksu\" = @studentID", new() { { "@studentID", studentId } });
            return new ExtendedStudentModel(result[0]);
        }

    }

}
