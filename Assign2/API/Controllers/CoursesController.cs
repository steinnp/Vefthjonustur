using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assign2.Models;

namespace Assign2.API.Controllers
{
  [Route("api/courses")]
  public class CoursesController : Controller
  {
    [HttpGet]
    public List<CourseLiteDTO> GetCoursesOnSemester(string semester = null)
    {
      return new List<CourseLiteDTO>
      {
        new CourseLiteDTO
        {
          ID = 1,
          Name = "Web Services",
          Semester = "20163"
        }
      };
    }
  }
}