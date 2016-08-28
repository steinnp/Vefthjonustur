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
    private static List<Student> _students;

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
            Students = new List<Student>{
              new Student{
                SSN = "3008942249",
                Name = "Janus"
              },
              new Student{
                SSN = "1209953279",
                Name = "Steinn"
              }
            }
          },
          new Course
          {
            Name = "Þýðendur",
            TemplateID = "T-513-THID",
            ID = 2,
            StartDate = new DateTime(2016,08,25),
            EndDate = new DateTime(2016,11,29),
            Students = new List<Student>{
              new Student{
                SSN = "3008942249",
                Name = "Janus"
              },
              new Student{
                SSN = "1209953279",
                Name = "Steinn"
              }
            }
          }
        };
      }
      if ( _courses == null )
      {
        _students = new List<Student>{
          new Student{
            SSN = "3008942249",
            Name = "Janus"
          },
          new Student{
            SSN = "1209953279",
            Name = "Steinn"
          },
          new Student{
            SSN = "1702931269",
            Name = "Sigurður"
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
    public IActionResult AddCourse ([FromBody]Course newCourse)
    {
      if (newCourse.Name == null ||
          newCourse.TemplateID == null ||
          newCourse.StartDate == null ||
          newCourse.EndDate == null)
      {
        string returnMessage = 
          "To add a course you must specify the following parameters:" +
          "Name, TemplateID, StartDate, EndDate";
        return BadRequest(returnMessage);
      }
      int newCourseID = _courses.Count() + 1;
      newCourse.ID = newCourseID;
      _courses.Add(newCourse);
      var location = Url.Link("GetCourse", new { ID = newCourseID });
      return Created(location, newCourse);
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
    public IActionResult UpdateCourse ([FromBody]Course updateCourse)
    {
      var Course = _courses.FirstOrDefault(x => x.ID == updateCourse.ID);
      if (Course == null)
      {
        string returnMessage = "Either the course ID was not specified or the course with"
                             + " the given ID could not be found.";
        return NotFound(returnMessage);
      }
      if (updateCourse.Name != null)
      {
        Course.Name = updateCourse.Name;
      }
      if (updateCourse.TemplateID != null)
      {
        Course.TemplateID = updateCourse.TemplateID;
      }
      if (updateCourse.StartDate != null)
      {
        Course.StartDate = updateCourse.StartDate;
      }
      if (updateCourse.EndDate != null)
      {
        Course.EndDate = updateCourse.EndDate;
      }
      var location = Url.Link("GetCourse", new { ID = updateCourse.ID });
      return Ok(location);
    }

    [HttpPost]
    [Route("/api/courses/{ID:int}/addStudent")]
    public IActionResult AddStudent (string ID, [FromBody]Student newStudent)
    {
      int CourseID = Int32.Parse(ID);

      Course TempCourse = _courses.FirstOrDefault(x => x.ID == CourseID);
      if(TempCourse == null){
        return NotFound("Given Course Not Found");
      }
      else{
        TempCourse.Students.Add(newStudent);
        var location = Url.Link("GetCourseStudents", new { ID = CourseID });
        return Created(location, newStudent);
      }
    }



    [HttpGet]
    [Route("api/courses/students/{ID:int}", Name="GetCourseStudents")]
    public IActionResult GetCourseStudents(string ID)
    {
      int CourseID = Int32.Parse(ID);
      Course TempCourse = _courses.FirstOrDefault(x => x.ID == CourseID);
      if(TempCourse == null){
        return NotFound();
      }
      else{
        return Ok(TempCourse.Students);
      }

    }
  }
}