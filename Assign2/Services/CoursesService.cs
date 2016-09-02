using System;
using System.Linq;
using System.Collections.Generic;
using Assign2.Models;

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
            return (from x in _db.Courses
            select new CourseLiteDTO{
                ID = x.ID,
                Name = x.Name,
                Semester = x.Semester
            }).ToList();
        }
    }
}
