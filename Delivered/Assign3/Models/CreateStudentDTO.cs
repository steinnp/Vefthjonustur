using System;
using System.Collections.Generic;

namespace Assign3.Models
{
  public class CreateStudentDTO
  {
   public string Name { get; set; }
   public int CourseID { get; set; }
   public bool Queued { get; set; }
  }
}
