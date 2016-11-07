using System;

namespace Assign3.Models
{
  /// <summary>
  /// A model for displaying information about courses
  /// </summary>  
  public class CourseLiteDTO
  {
   public int ID { get; set; } 
   public string Name { get; set; }
   public string Semester { get; set; }
   public string StartDate { get; set; }
   public string EndDate { get; set; }
   public int NumberOfStudents { get; set; }
  }
}
