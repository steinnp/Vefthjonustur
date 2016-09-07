using System;
using System.Linq;
using System.Collections.Generic;
using Assign2.Models;
using Assign2.Services.Entities;
using Assign2.Services.Exceptions;

namespace Assign2.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly AppDataContext _db;

        public CoursesService(AppDataContext db)
        {
            _db = db;
        }

        public List<CourseDetailsDTO> GetCoursesBySemester(string semester)
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
            if (coursesInSemester.Count() == 0)
            {
                throw new AppObjectNotFoundException();
            }
            return coursesInSemester;
        }

        // Done
        public CourseDetailsDTO GetCourseById(int id){

            List<StudentLiteDTO> students = (
                                from sl in _db.StudentLinker
                                join s in _db.Students on sl.StudentID equals s.ID
                                where sl.CourseID == id
                                select new StudentLiteDTO {
                                    SSN = s.SSN,
                                    Name = s.Name
                                }
            ).ToList();


            var course = (
                    from c in _db.Courses
                    join ct in _db.CoursesTemplate on c.CourseID equals ct.CourseID
                    where c.ID == id
                    select new CourseDetailsDTO {
                        ID = c.ID,
                        Name = ct.Name,
                        Semester = c.Semester,
                        StudentList = students

                    }).First();
            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }
            return course;
        }

        // Done
        // Judge me!
        public void PutCourseById(int Id, string CourseID, string StartDate, string EndDate){
            
            var course = (
                        from c in _db.Courses
                        where c.ID == Id
                        select c
                        ).First();

            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }
            if (StartDate != null)
            {        
               course.StartDate = StartDate;
            }
            if (EndDate != null)
            {
                course.EndDate = EndDate;
            }
            _db.SaveChanges();
        }

        /*public void DeleteCourseById(int Id) {
            var course = (
                from c in _db.Courses
                where c.ID == Id
                select c ).First();
            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }

            _db.Courses.Deete(course);
            _db.SaveChanges();
        }*/
    
    
    
    }
}
