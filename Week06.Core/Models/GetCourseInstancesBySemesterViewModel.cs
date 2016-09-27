using System.Collections.Generic;

namespace CoursesAPI.Models
{
	/// <summary>
	/// This class represents the data needed when 
	/// getting courses by semester
	/// </summary>
	public class GetCourseInstancesBySemesterViewModel
	{
        public List<CourseInstanceDTO> Items { get; set; }
        public PagingDTO Paging { get; set; }
    }
}