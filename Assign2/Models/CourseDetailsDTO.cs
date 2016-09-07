using System;
using System.Collections.Generic;

namespace Assign2.Models
{
    
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
