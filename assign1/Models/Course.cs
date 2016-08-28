using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Models
{
  public class Course
  {
    public int ID { get; set; }
    public string Name { get; set; }
    public string TemplateID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Student> Students { get; set; }
  }
}
