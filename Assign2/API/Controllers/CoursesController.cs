using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assign2.Models;
using Assign2.Services;
using Assign2.Services.Exceptions;

namespace Assign2.API.Controllers
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
    [Route("{id:int}")]
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
    public IActionResult PutCourseById(int id, [FromBody]CourseLiteDTO course)
    {
      try
      {
        _service.PutCourseById(id, course.StartDate, course.EndDate);
        return Ok();
      }
      catch (AppObjectNotFoundException)
      {
        return NotFound("No course was found with that ID");
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
    
    [HttpGet]
    [Route("{id:int}/students")]
    public IActionResult GetStudentsByCourseId(int id){
      try
      {
        return Ok(_service.GetStudentsByCourseId(id));
      }
      catch (System.Exception e)
      {
        return NotFound(e.Message);
      }
    }
  
  
  }
}