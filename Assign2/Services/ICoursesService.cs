using System;
using System.Collections.Generic;
using Assign2.Models;

namespace Assign2.Services
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

