using System;

namespace Assign3.Services.Exceptions
{
    public class AppObjectNotFoundException : Exception
    {
        public AppObjectNotFoundException()
        {

        }
        public AppObjectNotFoundException(string message) : base(message)
        {
        }
        public AppObjectNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }    
}