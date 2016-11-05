using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoursesAPI.Controllers
{
    [Route("api/courses")]
    public class CoursesController : Controller
    {
        [HttpGet]
        public String Get()
        {
            return "smuuu";
        }

        [HttpPost]
        public String Post()
        {
            return "smuuuu";
        }

        [HttpGet]
        [Route("{id}")]
        public String getId(int Id)
        {
            return "smuuu";
        }
    }
}