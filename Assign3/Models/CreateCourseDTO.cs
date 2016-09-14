using System;
using System.Collections.Generic;

namespace Assign3.Models
{
  /// <summary>
  /// A model for showing detailed information about a course.
  /// </summary>  
  public class CreateCourseDTO
  {
   public string TemplateID { get; set; }
   public string Semester { get; set; }
   public string StartDate { get; set; }
   public string EndDate { get; set; }
   public int MaxStudents { get; set; }
  }
}

