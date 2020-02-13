using System;
using System.Collections.Generic;

namespace Project.CommandProcessor.Utilities
{
    public class ExceptionParser
    {
        private readonly Exception _exception;

        public ExceptionParser(Exception ex)
        {
            _exception = ex;
        }

        /// <summary>
        /// Method will concatenate all the messages in Exception & its InnerException delimited by "->".
        /// </summary>
        /// <returns>Returns string which contains exception messages in Exception & its InnerException delimited by "->".</returns>
        public string GetMessages()
        {
            if (_exception == null)
            {
                return string.Empty;
            }

            var messages = new List<string>();

            messages.Add(_exception.Message);

            //if (typeof (FaultException<ExceptionDetail>) == _exception.GetType())
            //{
            //    CollectFaultExceptionMessages((FaultException<ExceptionDetail>) _exception, messages);
            //}
            //else
            //{
            //    CollectExceptionMessages(_exception, messages);
            //}
            messages.Reverse();
            ////Reversing here since the innermost exception is the one that caused the issue hence bringing it on top.
            return string.Join("->", messages);
        }

        //private static void CollectFaultExceptionMessages(FaultException<ExceptionDetail> fe, IList<string> messages)
        //{
        //    ExceptionDetail innerException = fe.Detail;

        //    while (innerException != null)
        //    {
        //        messages.Add(innerException.Message);
        //        innerException = innerException.InnerException;
        //    }
        //}

        private static void CollectExceptionMessages(Exception ex, IList<string> messages)
        {
            Exception innerException = ex.InnerException;

            while (innerException != null)
            {
                messages.Add(innerException.Message);
                innerException = innerException.InnerException;
            }
        }
    }
}
