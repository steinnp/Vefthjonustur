using System;
using System.Collections.Generic;
using Assign3.Models;

namespace Assign3.Services
{
    public interface ICoursesService
    {
        CreateStudentDTO AddStudentToCourse(int courseID, string ssn);
        List<CourseLiteDTO> GetCoursesBySemester(string semester);
        CourseDetailsDTO GetCourseById(int Id);
        CourseDetailsDTO CreateCourse(CreateCourseDTO course);
        void PutCourseById(int id, string StartDate, string EndDate, int MaxStudents);
        void DeleteCourseById(int Id);
        List<StudentLiteDTO> GetStudentsByCourseId(int id);
        List<StudentLiteDTO> GetStudentsWaitingByCourseId(int id);
        void DeleteStudentFromCourse(int courseID, string ssn);
    }
}

