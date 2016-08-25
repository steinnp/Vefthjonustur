using System;

namespace WebApplication.Models
{
  public class Course
  {
    public string Name { get; set; }
    public string TemplateID { get; set; }
    public int ID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }
}
