using System;

namespace RxGen.Core.Exceptions
{
    /// <summary>
    /// Rx gen api exception
    /// </summary>
    public class RxGenApiException : Exception
    {
        public RxGenApiException(Exception ex)
            : base("RxGenApi exception", ex)
        {
        }

        public RxGenApiException(string message)
            : base($"RxGenApi exception: {message}")
        {
        }

        public RxGenApiException(string message, Exception ex)
            : base($"RxGenApi exception: {message}", ex)
        {
        }
    }
}