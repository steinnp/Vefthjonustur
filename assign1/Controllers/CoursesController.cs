using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
  public class CoursesController : Controller
  {
    private static List<Course> _courses;

    public CoursesController()
    {
      if ( _courses == null )
      {
        _courses = new List<Course>
        {
          new Course
          {
            Name = "Vefthjonustur",
            TemplateID = "T-514-VEFT",
            ID = 1,
            StartDate = new DateTime(2016,08,25),
            EndDate = new DateTime(2016,11,29),
          },
          new Course
          {
            Name = "Þýðendur",
            TemplateID = "T-513-THID",
            ID = 2,
            StartDate = new DateTime(2016,08,25),
            EndDate = new DateTime(2016,11,29),
          }
        };
      }
    }

    [HttpGet]
    [Route("/api/courses")]
    public IActionResult Courses ()
    {
      if (_courses == null)
      {
        return NotFound();
      }
      return Ok(_courses);
    }

    [HttpGet]
    [Route("/api/courses/{ID:int}", Name = "GetCourse")]
    public IActionResult GetCourse(string ID)
    {
      int CourseID;
      try
      {
        CourseID = Int32.Parse(ID);
      } catch (FormatException) {
        return NotFound();
      }
      var ReturnCourse = _courses.FirstOrDefault(i => i.ID == CourseID);
      if (ReturnCourse == null) {
        return NotFound();
      }
      return Ok(ReturnCourse);
    }

    [HttpPost]
    [Route("/api/courses")]
    public IActionResult AddCourse  (string Name,
                                     string TemplateID,
                                     string StartDate,
                                     string EndDate
                                    )
    {
      if (Name == null || Name.GetType() != typeof(string) || TemplateID == null || StartDate == null || EndDate == null)
      {
        string returnMessage = 
          "To add a course you must specify the following parameters:" +
          "Name, TemplateID, StartDate, EndDate";
        return BadRequest(returnMessage);
      }
      DateTime StartTime;
      DateTime EndTime;
      try
      {
        StartTime = DateTime.Parse(StartDate);
      } catch (FormatException) {
        string returnMessage = "The StartDate parameter is not of a valid DateTime format";
        return BadRequest(returnMessage);
      }
      try
      {
        EndTime = DateTime.Parse(EndDate);
      } catch (FormatException) {
        string returnMessage = "The EndDate parameter is not of a valid DateTime format";
        return BadRequest(returnMessage);
      }
      int newCourseID = _courses.Count() + 1;
      Course NewCourse = new Course
                         {
                           Name = Name,
                           TemplateID = TemplateID,
                           ID = newCourseID,
                           StartDate = StartTime,
                           EndDate = EndTime,
                         };
      _courses.Add(NewCourse);
      var location = Url.Link("GetCourse", new { ID = newCourseID });
      return Created(location, NewCourse);
    }
    
    [HttpDelete]
    [Route("/api/courses/{ID:int}")]
    public IActionResult DeleteCourse(int ID)
    {
      int result = _courses.RemoveAll(x => x.ID == ID);
      if (result == 0)
      {
        return NotFound();
      }
      return NoContent();
    }

    [HttpPut]
    [Route("/api/courses/")]
    public IActionResult UpdateCourse (string Name,
                                       string TemplateID,
                                       string StartDate,
                                       string EndDate,
                                       string ID
                                      )
    {
      if (ID == null)
      {
        return BadRequest("To modify a course the course ID must be specified");
      }
      int CourseID;
      try
      {
        CourseID = Int32.Parse(ID);
      } catch (FormatException) {
        return NotFound();
      }
      var Course = _courses.FirstOrDefault(x => x.ID == CourseID);
      if (Course == null)
      {
        return NotFound();
      }
      if (Name != null)
      {
        Course.Name = Name;
      }
      if (TemplateID != null)
      {
        Course.TemplateID = TemplateID;
      }
      if (StartDate != null)
      {
        DateTime StartTime;
        try
        {
          StartTime = DateTime.Parse(StartDate);
        } catch (FormatException) {
          string returnMessage = "The StartDate parameter is not of a valid DateTime format";
          return BadRequest(returnMessage);
        }
        Course.StartDate = StartTime;
      }
      if (EndDate != null)
      {
        DateTime EndTime;
        try
        {
          EndTime = DateTime.Parse(EndDate);
        } catch (FormatException) {
          string returnMessage = "The EndDate parameter is not of a valid DateTime format";
          return BadRequest(returnMessage);
        }
        Course.EndDate = EndTime;
      }
      var location = Url.Link("GetCourse", new { ID = CourseID });
      return Ok(location);
    }
  }
}