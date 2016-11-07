using System;
using System.Collections.Generic;
using Assign3.Models;

namespace Assign3.Services
{
    public interface ICoursesService
    {
        void AddStudentToCourse(int courseID, string ssn);
        List<CourseLiteDTO> GetCoursesBySemester(string semester);
        CourseDetailsDTO GetCourseById(int Id);
        void PutCourseById(int id, string StartDate, string EndDate);
        void DeleteCourseById(int Id);
        List<StudentLiteDTO> GetStudentsByCourseId(int id);
    }
}

