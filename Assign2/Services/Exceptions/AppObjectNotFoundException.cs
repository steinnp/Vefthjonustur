using System;

namespace Assign3.Services.Exceptions
{
    public class AppObjectNotFoundException : Exception
    {
        public string message { get; set; }
        public AppObjectNotFoundException()
        {

        }
        public AppObjectNotFoundException(string message)
        {
            this.message = message;
        }
    }    
}