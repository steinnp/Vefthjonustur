using Microsoft.AspNetCore.Mvc;
using System;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Services;
using CoursesAPI.Services.Exceptions;

namespace CoursesAPI.Controllers
{
	[Route("api/courses")]
	public class CoursesController : Controller
	{
		private readonly CoursesServiceProvider _service;

		public CoursesController(IUnitOfWork uow)
		{
			_service = new CoursesServiceProvider(uow);
		}

		/// <summary>
        /// Gets a list of courses by semester id by language, accepts language headers
		/// is-IS, is, en-US, en. Anything else defaults to english.
        /// </summary>
        /// <param name="semester">The semester ID, defaults to "20153"</param>
        /// <param name="page">The page number requested, defaults to 1</param>
        /// <returns>200 Ok, an object with at most 10 results as well as a paging object with page info.
		/// Example: {
 		/// "items": [
 		///   {
 		///     "courseInstanceID": 1,
 		///     "templateID": "T-111-PROG",
	    ///    "name": "Forritun",
		///     "mainTeacher": ""
		///  },
   		/// {
    	///  "courseInstanceID": 2,
    	///  "templateID": "T-514-VEFT",
    	///  "name": "Vefþjónustur",
    	///  "mainTeacher": ""
  		///  }
 		/// ],
 		/// "paging": {
  		///  "pageCount": 1,
  		///  "pageSize": 10,
  		///  "pageNumber": 1,
  		///  "totalNumberOfItems": 2
 		/// }
		///}
		/// 404 not found if no results were found for the given page.
		/// <returns>
		[HttpGet]
		public IActionResult GetCoursesBySemester(string semester = null, int page = 1)
		{
			string language = Request.Headers["Accept-Language"];
			try
			{
				return Ok(_service.GetCourseInstancesBySemester(semester, page, language));
			}
			catch (AppObjectNotFoundException)
			{
				return NotFound("The page number entered does not match any results.");
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{id}/teachers")]
		public IActionResult AddTeacher(int id, AddTeacherViewModel model)
		{
			var result = _service.AddTeacherToCourse(id, model);
			return Created("TODO", result);
		}
	}
}
