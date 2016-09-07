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

        public string AddStudentToCourse(int courseID, string ssn = null)
        {
            if (ssn == null)
            {
                throw new AppObjectNotFoundException("No student was found with that ssn");
            }
            var course = (from c in _db.Courses
                          where c.ID == courseID
                          select c).SingleOrDefault();
            if (course == null)
            {
                throw new AppObjectNotFoundException("No course was found with that ID");
            }
            var student = (from s in _db.Students
                           where s.SSN == ssn
                           select s).SingleOrDefault();
            if (student == null)
            {
                throw new AppObjectNotFoundException("No student was found with that ssn");
            }
            return "";
        }

        public List<CourseLiteDTO> GetCoursesBySemester(string semester)
        {
            if (semester == null)
            {
                semester = "20163";
            }
            var courses = _db.Courses;
            var templates = _db.CoursesTemplate;
            var StudentLinker = _db.StudentLinker;
            
            
            
            var coursesInSemester = 
                (from c in courses
                join ct in templates on c.CourseID equals ct.CourseID
                where c.Semester == semester
                select new CourseLiteDTO {
                    ID = c.ID,
                    Name = ct.Name,
                    Semester = c.Semester,
                    NumberOfStudents = (
                        from sl in StudentLinker
                        where sl.CourseID == c.ID
                        select sl
                    ).Count(),
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                }).ToList();

            if (coursesInSemester.Count() == 0)
            {
                throw new AppObjectNotFoundException();
            }
            return coursesInSemester;
        }

        // Done
        public CourseDetailsDTO GetCourseById(int id){
            var course = (
                    from c in _db.Courses
                    join ct in _db.CoursesTemplate on c.CourseID equals ct.CourseID
                    where c.ID == id
                    select new CourseDetailsDTO {
                        ID = c.ID,
                        Name = ct.Name,
                        Semester = c.Semester,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        StudentList = (
                                from sl in _db.StudentLinker
                                join s in _db.Students on sl.StudentID equals s.SSN
                                where sl.CourseID == id
                                select new StudentLiteDTO {
                                    SSN = s.SSN,
                                    Name = s.Name
                                }).ToList()

                    }).SingleOrDefault();
            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }
            return course;
        }

        // Done
        public void PutCourseById(int id, string StartDate = null, string EndDate = null){
            
            var course = (
                        from c in _db.Courses
                        where c.ID == id
                        select c
                        ).SingleOrDefault();

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

        public void DeleteCourseById(int Id) {
            Course course = (
                from c in _db.Courses
                where c.ID == Id
                select c).SingleOrDefault();

            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }

            _db.Courses.Remove(course);
            _db.SaveChanges();
        }
    }
}
