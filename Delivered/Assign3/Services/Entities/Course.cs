namespace Assign3.Services.Entities
{
    public class Course
    {
        public int ID { get; set; }
        public string CourseID { get; set; }
        public string Semester { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int MaxStudents { get; set; }
    }
}
