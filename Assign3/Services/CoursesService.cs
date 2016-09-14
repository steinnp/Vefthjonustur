using System;
using System.Linq;
using System.Collections.Generic;
using Assign3.Models;
using Assign3.Services.Entities;
using Assign3.Services.Exceptions;

namespace Assign3.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly AppDataContext _db;

        public CoursesService(AppDataContext db)
        {
            _db = db;
        }

        private Course GetCourse(int courseID)
        {
            var course = (from c in _db.Courses
                          where c.ID == courseID
                          select c).SingleOrDefault();
            if (course == null)
            {
                throw new AppObjectNotFoundException("No course was found with that ID");
            }
            return course;
        }
        private Student GetStudent(string ssn)
        {
            if (ssn == null)
            {
                throw new AppObjectNotFoundException("No student was found with that ssn");
            }
            var student = (from s in _db.Students
                           where s.SSN == ssn
                           select s).SingleOrDefault();
            if (student == null)
            {
                throw new AppObjectNotFoundException("No student was found with that ssn");
            }
            return student;
        }

        private void CourseStudentLinkExistsThrower(int courseID = 0, string ssn = null)
        {
            if (ssn == null || courseID == 0)
            {
                return;
            }
            var studentLink = (from c in _db.StudentLinker
                                where c.StudentID == ssn && c.CourseID == courseID && c.IsActive == 1
                                select c).SingleOrDefault();
            if (studentLink != null) {
                throw new InvalidParametersException("This student is already enrolled in the course");
            }
            var waitingLink = (from c in _db.WaitingList
                               where c.StudentID == ssn && c.CourseID == courseID
                               select c).SingleOrDefault();
            if (waitingLink != null) {
                throw new InvalidParametersException("This student is already in the waiting list for the course");
            }
        }
        private bool CourseIsFull(int courseID)
        {
            var course = (from c in _db.Courses
                          where c.ID == courseID
                          select c).SingleOrDefault();
            int numberOfStudents = (from c in _db.StudentLinker where c.CourseID == courseID && c.IsActive == 1 select c).Count();
            return numberOfStudents >= course.MaxStudents;
        }

        private DateTime DateTimeConverter(string time = null, string message = null)
        {
            if (time != null)
            {        
                DateTime timeFormatted;
                try {
                    timeFormatted = Convert.ToDateTime(time);
                } catch (FormatException) {
                    throw new InvalidParametersException(message);
                }
                return timeFormatted;
            } else {
                throw new InvalidParametersException(message);
            }

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
                        where sl.CourseID == c.ID && sl.IsActive == 1
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
                        MaxStudents = c.MaxStudents,
                        StudentList = (
                                from sl in _db.StudentLinker
                                join s in _db.Students on sl.StudentID equals s.SSN
                                where sl.CourseID == id && sl.IsActive == 1
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
        public void PutCourseById(int id, string StartDate = null, string EndDate = null, int MaxStudents = 0){
            var course = GetCourse(id);
            if (StartDate != null)
            {        
                DateTime startTime = DateTimeConverter(StartDate, "StartDate is not of a valid format.");
                if (Convert.ToDateTime(course.EndDate) < startTime) {
                    throw new InvalidParametersException("EndDate can not come before StartDate");
                }
               course.StartDate = StartDate;
            }
            if (EndDate != null)
            {
                DateTime endTime = DateTimeConverter(EndDate, "EndDate is not of a valid format.");
                if (Convert.ToDateTime(course.StartDate) > endTime) {
                    throw new InvalidParametersException("EndDate can not come before StartDate");
                }
                course.EndDate = EndDate;
            }
            if (MaxStudents > 0)
            {
                course.MaxStudents = MaxStudents;
            }
            _db.SaveChanges();
        }

        public CourseDetailsDTO CreateCourse(CreateCourseDTO course) {
            Course newCourse = new Course();
            if (course.TemplateID == null ||
                course.Semester == null ||
                course.StartDate == null ||
                course.EndDate == null ||
                course.MaxStudents < 1
               ) {
                throw new InvalidParametersException("The following parameters need to be defined:" +
                                                     "\nTemplateID" +
                                                     "\nSemester" + 
                                                     "\nStartDate" +
                                                     "\nEndDate" +
                                                     "\nMaxStudents"
                                                    );
            }
            DateTime startTime = DateTimeConverter(course.StartDate, "StartDate is not of a valid format.");
            DateTime endTime = DateTimeConverter(course.EndDate, "EndDate is not of a valid format.");
            if (endTime < startTime) {
                throw new InvalidParametersException("EndDate can not come before StartDate");
            }
            newCourse.CourseID = course.TemplateID;
            newCourse.Semester = course.Semester;
            newCourse.StartDate = course.StartDate;
            newCourse.EndDate = course.EndDate;
            newCourse.MaxStudents = course.MaxStudents;
            _db.Courses.Add(newCourse);
            _db.SaveChanges();
            return GetCourseById(newCourse.ID);
        }

        public void DeleteCourseById(int Id) {
            Course course = GetCourse(Id);
            _db.Courses.Remove(course);
            _db.SaveChanges();
        }
        public CreateStudentDTO AddStudentToCourse(int courseID, string ssn = null)
        {
            GetCourse(courseID);
            Student student = GetStudent(ssn);
            CourseStudentLinkExistsThrower(courseID, ssn);
            if (CourseIsFull(courseID)) {
                var studentLink = new WaitingList {
                    CourseID = courseID,
                    StudentID = ssn
                };
                _db.WaitingList.Add(studentLink);
                _db.SaveChanges();
                return new CreateStudentDTO {Name = student.Name, CourseID = courseID, Queued = true};
            } else {
                var studentLink = new StudentLinker {
                    CourseID = courseID,
                    StudentID = ssn,
                    IsActive = 1
                };
                _db.StudentLinker.Add(studentLink);
                _db.SaveChanges();
                return new CreateStudentDTO {Name = student.Name, CourseID = courseID, Queued = false};
            }
        }

        public void DeleteStudentFromCourse(int courseID, string ssn = null)
        {
            GetCourse(courseID);
            GetStudent(ssn);
            var studentLink = (from c in _db.StudentLinker
                                where c.StudentID == ssn && c.CourseID == courseID && c.IsActive == 1
                                select c).SingleOrDefault();
            var waitingLink = (from c in _db.WaitingList
                               where c.StudentID == ssn && c.CourseID == courseID
                               select c).SingleOrDefault();
            if (studentLink == null && waitingLink == null) {
                throw new AppObjectNotFoundException("The student is not enrolled in this course.");
            }
            if (waitingLink != null) {
                _db.WaitingList.Remove(waitingLink);
                _db.SaveChanges();
                return;
            }
            if (studentLink != null) {
                studentLink.IsActive = 0;
            }
            var firstInQueue = (
                from w in _db.WaitingList
                where w.CourseID == courseID
                select w
            ).FirstOrDefault();
            if (firstInQueue != null)
            {
                _db.StudentLinker.Add(new StudentLinker{CourseID = courseID, StudentID = firstInQueue.StudentID, IsActive = 1});
                _db.WaitingList.Remove(firstInQueue);
            }
            _db.SaveChanges();
        }

        public List<StudentLiteDTO> GetStudentsByCourseId(int id){
            GetCourse(id);
            var list = (
                from s in _db.Students
                join sl in _db.StudentLinker on s.SSN equals sl.StudentID 
                where sl.CourseID == id && sl.IsActive == 1
                select new StudentLiteDTO {
                    SSN = s.SSN,
                    Name = s.Name
                }
            ).ToList();
            if (list.Count() <= 0){
                // No students message
                throw new AppObjectNotFoundException("No Students In Course");
            }
            return list;
        }
        public List<StudentLiteDTO> GetStudentsWaitingByCourseId(int id){
            GetCourse(id);
            var list = (
                from s in _db.Students
                join sl in _db.WaitingList on s.SSN equals sl.StudentID 
                where sl.CourseID == id
                select new StudentLiteDTO {
                    SSN = s.SSN,
                    Name = s.Name
                }
            ).ToList();
            if (list.Count() <= 0){
                // No students message
                throw new AppObjectNotFoundException("No Students in the waiting list");
            }
            return list;
        }
    }
}
