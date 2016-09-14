using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assign3.Models;
using Assign3.Services;
using Assign3.Services.Exceptions;

namespace Assign3.API.Controllers
{
  [Route("api/courses")]
  public class CoursesController : Controller
  {
    private readonly ICoursesService _service;

    public CoursesController(ICoursesService service)
    {
      _service = service;
    }

    /// <summary>
    /// Gets a list of courses by semester
    /// Example 1: "/api/courses"
    /// returns the courses of the current semester
    /// Example 2: "/api/courses?semester=20153
    /// returns the courses of the 20153 semester
    /// </summary>
    /// <param name="semester">The semester to select courses from</param>
    /// <returns>200 Ok: The courses of the semester
    ///or the courses of the current semester 
    /// if no semester is specified
    /// 404 Not Found: No course can be found in the 
    /// specified semester
    ///</returns>
    [HttpGet]
    public IActionResult GetCoursesOnSemester(string semester = null)
    {
      try
      {
        var result = _service.GetCoursesBySemester(semester);
        return Ok(result);
      } catch (AppObjectNotFoundException) {
        return NotFound("No courses were found for the specified semester");
      }
    }

    /// <summary>
    /// Gets a list of courses by id
    /// Example: "/api/courses/1"
    /// returns the course with id 1 
    /// </summary>
    /// <param name="id">id of the course</param>
    /// <returns>200 Ok: The course object with the given id, 404 not found: no course was found with the given id</returns>
    [HttpGet]
    [Route("{id:int}", Name="GetCourseById")]
    public IActionResult GetCoursesById(int id) {
      try {
        return Ok(_service.GetCourseById(id));
      }
      catch (AppObjectNotFoundException) {
        return NotFound("No course was found with that ID");
      }
    }

    /// <summary>
    /// Modifies a course with a given id.
    /// Example: url: /api/courses/1,
    /// body:
    /// {
    /// "StartDate" : "20/01/2011"  
    /// }
    /// modifies the StartDate of course with id 1 to be 20/01/2011
    /// </summary>
    /// <param name="id">id of the course to be modified</param>
    /// <param name="course">the course object to be modified</param>
    /// <returns>200 ok if the update was succesful, 404 not found if no course with that id could be found</returns>
    [HttpPut]
    [Route("{id:int}")]
    public IActionResult PutCourseById(int id, [FromBody]CourseDetailsDTO course)
    {
      try
      {
        _service.PutCourseById(id, course.StartDate, course.EndDate, course.MaxStudents);
        return Ok();
      }
      catch (AppObjectNotFoundException)
      {
        return NotFound("No course was found with that ID");
      } catch (InvalidParametersException e) {
        return BadRequest(e.Message);
      }
    }

    /// <summary>
    /// Creates a new course
    /// Example:
    /// Header: POST /api/courses
    /// Body {
    ///   "TemplateID": "T-514-VEFT",
	  ///   "Semester": "postSemester",
	  ///   "StartDate": "2016/08/08",
	  ///   "EndDate": "2016/09/08",
	  ///   "MaxStudents": 5
    /// }
    /// </summary>
    /// <param name="course">Details of the course to be added</param>
    /// <returns>201 Created for a succesfully created course, 400 Bad request if not</returns>
    [HttpPost]
    public IActionResult CreateCourse([FromBody]CreateCourseDTO course)
    {
      try
      {
        CourseDetailsDTO newCourse = _service.CreateCourse(course);
        var location = Url.Link("GetCourseById", new { id = newCourse.ID });
        return Created(location, newCourse);
      } catch(InvalidParametersException e) {
        return BadRequest(e.Message);
      }
    }
    /// <summary>
    /// Deletes the course with the given id
    /// </summary>
    /// <param name="id">id of the course to be deleted</param>
    /// <returns>200 ok if the course was deleted succesfully, 404 not found if no course was found with the given id</returns>
    [HttpDelete]
    [Route("{id:int}")]
    public IActionResult DeleteCourseById(int id) {
      try
      {
        _service.DeleteCourseById(id);
        return Ok();
      } catch (AppObjectNotFoundException) {
        return NotFound("No course was found with that ID");
      }

    }

    [HttpDelete]
    [Route("{id:int}/students/{SSN}")]
    public IActionResult DeleteStudentFromCourse(int id, string SSN)
    {
      try
      {
        _service.DeleteStudentFromCourse(id, SSN);
        return NoContent();
      } catch (AppObjectNotFoundException e) {
        return NotFound(e.Message);
      }
    }
    
    /// <summary>
    /// Adds a student to a course 
    /// Example: url: /api/courses/1/students
    /// body: { "SSN": "1234567890" }
    /// adds student with ssn 1234567890 to course with id 1 
    /// </summary>
    /// <param name="courseID">The id of the course</param>
    /// <param name="student">contains the ssn of the student</param>
    /// <returns>201 created if the student was succesfully added, 404 not found if no student can be found with that ssn or no course can be found with that id</returns>
    [HttpPost]
    [Route("{courseID:int}/students")]
    public IActionResult AddStudentToCourse(int courseID, [FromBody]StudentLiteDTO student)
    {
      try
      {
        var newStudent = _service.AddStudentToCourse(courseID, student.SSN);
        if (newStudent.Queued == false) {
          var location = Url.Link("GetStudentsByCourseId", new { id = newStudent.CourseID });
          return Created(location, newStudent.Name + " is now enrolled in the course.");
        } else {
          var location = Url.Link("GetStudentsWaitingByCourseId", new { id = newStudent.CourseID });
          return Created(location, newStudent.Name + " has been added to the course waiting list");
        }
      } catch (AppObjectNotFoundException e){
        return NotFound(e.Message);
      } catch (InvalidParametersException) {
        var returnCode = new StatusCodeResult(412);
        return returnCode;
      }
    }

    /// <summary>
    /// Adds a student to a courses waiting list 
    /// Example: url: /api/courses/1/students
    /// body: { "SSN": "1234567890" }
    /// adds student with ssn 1234567890 to course with id 1 
    /// </summary>
    /// <param name="courseID">The id of the course</param>
    /// <param name="student">contains the ssn of the student</param>
    /// <returns>201 created if the student was succesfully added, 404 not found if no student can be found with that ssn or no course can be found with that id</returns>
    [HttpPost]
    [Route("{courseID:int}/waitinglist")]
    public IActionResult AddStudentToCourseWaitingList(int courseID, [FromBody]StudentLiteDTO student)
    {
      try
      {
        var newStudent = _service.AddStudentToCourse(courseID, student.SSN);
        if (newStudent.Queued == false) {
          var location = Url.Link("GetStudentsByCourseId", new { id = newStudent.CourseID });
          return Created(location, newStudent.Name + " is now enrolled in the course.");
        } else {
          var location = Url.Link("GetStudentsWaitingByCourseId", new { id = newStudent.CourseID });
          return Created(location, newStudent.Name + " has been added to the course waiting list");
        }
      } catch (AppObjectNotFoundException e){
        return NotFound(e.Message);
      } catch (InvalidParametersException) {
        var returnCode = new StatusCodeResult(412);
        return returnCode;
      }
    }


    /// <summary>
    /// Get all students from a course 
    /// Example: url: /api/courses/1/students
    /// Gets students from a given course using the course id as a identifier 
    /// </summary>
    /// <param name="courseID">The id of the course</param>
    /// <returns>Returns List of students or a 404 error if no course is found or no students belong to a course</returns>
    
    [HttpGet]
    [Route("{id:int}/students", Name="GetStudentsByCourseId")]
    public IActionResult GetStudentsByCourseId(int id){
      try
      {
        return Ok(_service.GetStudentsByCourseId(id));
      }
      catch (AppObjectNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    /// <summary>
    /// Get all students waiting to get into a course 
    /// Example: url: /api/courses/1/waitinglist
    /// Getss students from a given course using the course id as a identifier 
    /// </summary>
    /// <param name="courseID">The id of the course</param>
    /// <returns>Returns List of students or a 404 error if no course is found or no students belong to a course</returns>
    
    [HttpGet]
    [Route("{id:int}/waitinglist", Name="GetStudentsWaitingByCourseId")]
    public IActionResult GetStudentsWaitingByCourseId(int id){
      try
      {
        return Ok(_service.GetStudentsWaitingByCourseId(id));
      }
      catch (AppObjectNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }
  
  
  }
}