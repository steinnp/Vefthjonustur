using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;

namespace CoursesAPI.Services.Services
{
	public class CoursesServiceProvider
	{
		private readonly IUnitOfWork _uow;

		private readonly IRepository<CourseInstance> _courseInstances;
		private readonly IRepository<TeacherRegistration> _teacherRegistrations;
		private readonly IRepository<CourseTemplate> _courseTemplates; 
		private readonly IRepository<Person> _persons;

		public CoursesServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;

			_courseInstances      = _uow.GetRepository<CourseInstance>();
			_courseTemplates      = _uow.GetRepository<CourseTemplate>();
			_teacherRegistrations = _uow.GetRepository<TeacherRegistration>();
			_persons              = _uow.GetRepository<Person>();
		}

		/// <summary>
		/// You should implement this function, such that all tests will pass.
		/// </summary>
		/// <param name="courseInstanceID">The ID of the course instance which the teacher will be registered to.</param>
		/// <param name="model">The data which indicates which person should be added as a teacher, and in what role.</param>
		/// <returns>Should return basic information about the person.</returns>
		public PersonDTO AddTeacherToCourse(int courseInstanceID, AddTeacherViewModel model)
		{
			// TODO: implement this logic!
			return null;
		}

		/// <summary>
		/// You should write tests for this function. You will also need to
		/// modify it, such that it will correctly return the name of the main
		/// teacher of each course.
		/// </summary>
		/// <param name="semester"></param>
		/// <returns></returns>
		public GetCourseInstancesBySemesterViewModel GetCourseInstancesBySemester(string semester = null, int page = 1, string language = null)
		{
			if (string.IsNullOrEmpty(semester))
			{
				semester = "20153";
			}
			int coursesCount = (from c in _courseInstances.All()
				where c.SemesterID == semester
				select c).Count();

			if ((page -1) * 10 < coursesCount)
			{
				List<CourseInstanceDTO> courses;
				if (language == "is-IS" || language == "is")
				{
			    	courses = (from c in _courseInstances.All()
					join ct in _courseTemplates.All() on c.CourseID equals ct.CourseID
					where c.SemesterID == semester
					select new CourseInstanceDTO
					{
						Name               = ct.Name,
						TemplateID         = ct.CourseID,
						CourseInstanceID   = c.ID,
						MainTeacher        = "" // Hint: it should not always return an empty string!
					}).Skip((page - 1) * 10).Take(10).ToList();
				} else {
			    	courses = (from c in _courseInstances.All()
					join ct in _courseTemplates.All() on c.CourseID equals ct.CourseID
					where c.SemesterID == semester
					select new CourseInstanceDTO
					{
						Name               = ct.NameEN,
						TemplateID         = ct.CourseID,
						CourseInstanceID   = c.ID,
						MainTeacher        = "" // Hint: it should not always return an empty string!
					}).Skip((page - 1) * 10).Take(10).ToList();

				}

			foreach (var course in courses)
			{
				var teacherSSN = (from t in _teacherRegistrations.All()
					where t.CourseInstanceID == course.CourseInstanceID &&
					t.Type == TeacherType.MainTeacher
					select t.SSN
				).SingleOrDefault();
				if (teacherSSN != null)
				{
					var teacherName = (from n in _persons.All()
						where n.SSN == teacherSSN
						select n.Name
					).SingleOrDefault();
					if (teacherName != null)
					{
						course.MainTeacher = teacherName;
					}
				}	
			}
			int PageCount = coursesCount / 10;
			if (coursesCount % 10 != 0)
			{
				PageCount += 1;
			}	
			PagingDTO pageInfo = new PagingDTO {
				PageCount = PageCount,
				PageSize = 10,
				PageNumber = page,
				TotalNumberOfItems = coursesCount
			};

				return new GetCourseInstancesBySemesterViewModel {
					Items = courses,
					Paging = pageInfo
				};
			} else {
				throw new AppObjectNotFoundException();
			}
		}
	}
}
