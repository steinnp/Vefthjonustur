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
  }
}