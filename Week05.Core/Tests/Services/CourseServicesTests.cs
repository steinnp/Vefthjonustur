using System;
using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;
using CoursesAPI.Services.Services;
using CoursesAPI.Tests.MockObjects;
using Xunit;

namespace CoursesAPI.Tests.Services
{
	public class CourseServicesTests
	{
		private MockUnitOfWork<MockDataContext> _mockUnitOfWork;
		private CoursesServiceProvider _service;
		private List<TeacherRegistration> _teacherRegistrations;

		private const string SSN_DABS    = "1203735289";
		private const string SSN_GUNNA   = "1234567890";
		private const string INVALID_SSN = "9876543210";

		private const string NAME_GUNNA  = "Guðrún Guðmundsdóttir";
		private const string NAME_DABS = "Daníel B. Sigurgeirsson";

		private const int COURSEID_VEFT_20153 = 1337;
		private const int COURSEID_VEFT_20163 = 1338;
		private const int COURSEID_FORR_20163 = 1339;
		private const int COURSEID_VEFT_20183 = 1340;
		private const int COURSEID_FORR_20183 = 1341;
		private const int INVALID_COURSEID    = 9999;

		public CourseServicesTests()
		{
			_mockUnitOfWork = new MockUnitOfWork<MockDataContext>();

			#region Persons
			var persons = new List<Person>
			{
				// Of course I'm the first person,
				// did you expect anything else?
				new Person
				{
					ID    = 1,
					Name  = NAME_DABS,
					SSN   = SSN_DABS,
					Email = "dabs@ru.is"
				},
				new Person
				{
					ID    = 2,
					Name  = NAME_GUNNA,
					SSN   = SSN_GUNNA,
					Email = "gunna@ru.is"
				}
			};
			#endregion

			#region Course templates

			var courseTemplates = new List<CourseTemplate>
			{
				new CourseTemplate
				{
					CourseID    = "T-514-VEFT",
					Description = "Í þessum áfanga verður fjallað um vefþj...",
					Name        = "Vefþjónustur"
				},
				new CourseTemplate
				{
					CourseID    = "T-101-FORR",
					Description = "Í þessum áfanga verður fjallað um forr...",
					Name        = "Forritun"
				}
			};
			#endregion

			#region Courses
			var courses = new List<CourseInstance>
			{
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20153,
					CourseID   = "T-514-VEFT",
					SemesterID = "20153"
				},
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20163,
					CourseID   = "T-514-VEFT",
					SemesterID = "20163"
				},
				new CourseInstance
				{
					ID         = COURSEID_FORR_20163,
					CourseID   = "T-101-FORR",
					SemesterID = "20163"
				},
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20183,
					CourseID   = "T-514-VEFT",
					SemesterID = "20183"
				},
				new CourseInstance
				{
					ID         = COURSEID_FORR_20183,
					CourseID   = "T-101-FORR",
					SemesterID = "20183"
				}
			};
			#endregion

			#region Teacher registrations
			_teacherRegistrations = new List<TeacherRegistration>
			{
				new TeacherRegistration
				{
					ID               = 101,
					CourseInstanceID = COURSEID_VEFT_20153,
					SSN              = SSN_DABS,
					Type             = TeacherType.MainTeacher
				},
				new TeacherRegistration
				{
					ID               = 102,
					CourseInstanceID = COURSEID_VEFT_20183,
					SSN              = SSN_DABS,
					Type             = TeacherType.MainTeacher
				},
				new TeacherRegistration
				{
					ID               = 103,
					CourseInstanceID = COURSEID_FORR_20183,
					SSN              = SSN_GUNNA,
					Type             = TeacherType.MainTeacher
				},
			};
			#endregion

			_mockUnitOfWork.SetRepositoryData(persons);
			_mockUnitOfWork.SetRepositoryData(courseTemplates);
			_mockUnitOfWork.SetRepositoryData(courses);
			_mockUnitOfWork.SetRepositoryData(_teacherRegistrations);

			// TODO: this would be the correct place to add 
			// more mock data to the mockUnitOfWork!

			_service = new CoursesServiceProvider(_mockUnitOfWork);
		}

		#region GetCoursesBySemester
		/// <summary>
		/// TODO: implement this test, and several others!
		/// </summary>
		[Fact]
		public void GetCoursesBySemester_ReturnsEmptyListWhenNoDataDefined()
		{
			MockUnitOfWork<MockDataContext> emptyMockUnitOfWork = new MockUnitOfWork<MockDataContext>();
			CoursesServiceProvider emptyService = new CoursesServiceProvider(emptyMockUnitOfWork);
			List<CourseInstanceDTO> result = emptyService.GetCourseInstancesBySemester();
			// Assert:
			Assert.True(result.Count() == 0);
		}

		[Fact]
		void GetCoursesBySemester_ReturnsAllCoursesOnSemester()
		{
			List<CourseInstanceDTO> result20153 = _service.GetCourseInstancesBySemester("20153");
			List<CourseInstanceDTO> result20163 = _service.GetCourseInstancesBySemester("20163");
			List<CourseInstanceDTO> result20173 = _service.GetCourseInstancesBySemester("20173");
			Assert.True(result20153.Count() == 1);
			Assert.True(result20153.Any(course => course.TemplateID == "T-514-VEFT"));
			Assert.True(result20163.Count() == 2);
			Assert.True(result20163.Any(course => course.TemplateID == "T-514-VEFT"));
			Assert.True(result20163.Any(course => course.TemplateID == "T-101-FORR"));
			Assert.True(result20173.Count() == 0);

		}

		[Fact]
		void GetCourseInstancesBySemester_ReturnsAllCoursesForUnspecifiedSemester()
		{
			List<CourseInstanceDTO> result = _service.GetCourseInstancesBySemester();
			Assert.True(result.Count() == 1);
			Assert.True(result.Any(course => course.TemplateID == "T-514-VEFT"));
		}

		[Fact]
		void GetCourseInstancesBySemester_ReturnsEmptyStringForNoMainTeacher()
		{
			List<CourseInstanceDTO> result = _service.GetCourseInstancesBySemester("20163");
			CourseInstanceDTO noMainTeacher = result.SingleOrDefault(course => course.TemplateID == "T-101-FORR");
			Assert.True(noMainTeacher.MainTeacher == "");
		}

		[Fact]
		void GetCourseInstancesBySemester_ReturnsMainTeacherNameIfDefined()
		{
			List<CourseInstanceDTO> result20153 = _service.GetCourseInstancesBySemester("20153");
			CourseInstanceDTO MainTeacher20153 = result20153.SingleOrDefault(course => course.TemplateID == "T-514-VEFT");
			Assert.True(MainTeacher20153.MainTeacher == NAME_DABS);
			List<CourseInstanceDTO> result20183 = _service.GetCourseInstancesBySemester("20183");
			CourseInstanceDTO MainTeacher20183VEFT = result20183.SingleOrDefault(course => course.TemplateID == "T-514-VEFT");
			Assert.True(MainTeacher20183VEFT.MainTeacher == NAME_DABS);
			CourseInstanceDTO MainTeacher20183FORR = result20183.SingleOrDefault(course => course.TemplateID == "T-101-FORR");
			Assert.True(MainTeacher20183FORR.MainTeacher == NAME_GUNNA);
		}

		#endregion
		#region AddTeacher

		/// <summary>
		/// Adds a main teacher to a course which doesn't have a
		/// main teacher defined already (see test data defined above).
		/// </summary>
		[Fact]
		public void AddTeacher_WithValidTeacherAndCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			var prevCount = _teacherRegistrations.Count;
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			var dto = _service.AddTeacherToCourse(COURSEID_VEFT_20163, model);

			// Assert:

			// Check that the dto object is correctly populated:
			Assert.Equal(SSN_GUNNA, dto.SSN);
			Assert.Equal(NAME_GUNNA, dto.Name);

			// Ensure that a new entity object has been created:
			var currentCount = _teacherRegistrations.Count;
			Assert.Equal(prevCount + 1, currentCount);

			// Get access to the entity object and assert that
			// the properties have been set:
			var newEntity = _teacherRegistrations.Last();
			Assert.Equal(COURSEID_VEFT_20163, newEntity.CourseInstanceID);
			Assert.Equal(SSN_GUNNA, newEntity.SSN);
			Assert.Equal(TeacherType.MainTeacher, newEntity.Type);

			// Ensure that the Unit Of Work object has been instructed
			// to save the new entity object:
			Assert.True(_mockUnitOfWork.GetSaveCallCount() > 0);
		}

		[Fact]
//		[ExpectedException(typeof(AppObjectNotFoundException))]
		public void AddTeacher_InvalidCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.AssistantTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			Assert.Throws<AppObjectNotFoundException>( () => _service.AddTeacherToCourse(INVALID_COURSEID, model) );
		}

		/// <summary>
		/// Ensure it is not possible to add a person as a teacher
		/// when that person is not registered in the system.
		/// </summary>
		[Fact]
//		[ExpectedException(typeof (AppObjectNotFoundException))]
		public void AddTeacher_InvalidTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = INVALID_SSN,
				Type = TeacherType.MainTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			Assert.Throws<AppObjectNotFoundException>( () => _service.AddTeacherToCourse(COURSEID_VEFT_20153, model));
		}

		/// <summary>
		/// In this test, we test that it is not possible to
		/// add another main teacher to a course, if one is already
		/// defined.
		/// </summary>
		[Fact]
		//[ExpectedExceptionWithMessage(typeof (AppValidationException), "COURSE_ALREADY_HAS_A_MAIN_TEACHER")]
		public void AddTeacher_AlreadyWithMainTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			Exception ex = Assert.Throws<AppValidationException>( () => _service.AddTeacherToCourse(COURSEID_VEFT_20153, model));
			Assert.Equal(ex.Message, "COURSE_ALREADY_HAS_A_MAIN_TEACHER");
		}

		/// <summary>
		/// In this test, we ensure that a person cannot be added as a
		/// teacher in a course, if that person is already registered
		/// as a teacher in the given course.
		/// </summary>
		[Fact]
		// [ExpectedExceptionWithMessage(typeof (AppValidationException), "PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE")]
		public void AddTeacher_PersonAlreadyRegisteredAsTeacherInCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_DABS,
				Type = TeacherType.AssistantTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			Exception ex = Assert.Throws<AppValidationException>( () => _service.AddTeacherToCourse(COURSEID_VEFT_20153, model));
			Assert.Equal(ex.Message, "PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE");
		}

		#endregion
	}
}
