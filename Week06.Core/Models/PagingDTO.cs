namespace CoursesAPI.Models
{
	/// <summary>
    /// An object for representing paging when getting courses by semester.
	///  </summary>
	public class PagingDTO
    {
        /// <summary>
        /// The number of pages
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// The number of items in each page 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// A 1-based index of the current page being returned 
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// The total number of items in the collection 
        /// </summary>
        public int TotalNumberOfItems { get; set; }
    }
}
