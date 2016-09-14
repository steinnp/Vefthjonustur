using System;
using System.Collections.Generic;

namespace Assign3.Models
{
  /// <summary>
  /// A model for showing detailed information about a course.
  /// </summary>  
  public class CourseDetailsDTO
  {
   public int ID { get; set; } 
   public string Name { get; set; }
   public string Semester { get; set; }
   public string StartDate { get; set; }
   public string EndDate { get; set; }
   public List<StudentLiteDTO> StudentList { get; set; }
  }
}
