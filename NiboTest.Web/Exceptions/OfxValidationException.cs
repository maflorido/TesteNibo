using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiboTest.Web.Exceptions
{
    public class OfxValidationException : Exception
    {
        public OfxValidationException()
            : base()
        {

        }

        public OfxValidationException(string message)
        : base(message)
        {
        }

        public OfxValidationException(string message, Exception inner)
        : base(message, inner)
        {

        }
    }
}