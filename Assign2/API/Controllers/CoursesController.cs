using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assign2.Models;
using Assign2.Services;

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

    [HttpGet]
    public IActionResult GetCoursesOnSemester(string semester = null)
    {
      var result = _service.GetCoursesBySemester(semester);
      return Ok(result);
      /*
      return new List<CourseLiteDTO>
      {
        new CourseLiteDTO
        {
          ID = 1,
          Name = "Web Services",
          Semester = "20163"
        }
      };
      */
    }
  }
}