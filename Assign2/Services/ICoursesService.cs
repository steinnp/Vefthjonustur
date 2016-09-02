using System;
using System.Collections.Generic;
using Assign2.Models;

namespace Assign2.Services
{
    public interface ICoursesService
    {
        List<CourseLiteDTO> GetCoursesBySemester(string semester);
    }
}

