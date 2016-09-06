using System;
using System.Linq;
using System.Collections.Generic;
using Assign2.Models;
using Assign2.Services.Entities;

namespace Assign2.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly AppDataContext _db;

        public CoursesService(AppDataContext db)
        {
            _db = db;
        }
        public List<CourseLiteDTO> GetCoursesBySemester(string semester)
        {
            if (semester == null)
            {
                semester = "20163";
            }
            var courses = _db.Courses;
            var templates = _db.CoursesTemplate;
            var coursesInSemester = 
                (from c in courses
                join ct in templates on c.CourseID equals ct.CourseID
                where c.Semester == semester
                select new CourseLiteDTO {
                    ID = c.ID,
                    Name = ct.Name,
                    Semester = c.Semester
                }).ToList();
            return coursesInSemester;
        }
    }
}
