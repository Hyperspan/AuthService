namespace AuthServer.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ApiErrorException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        public ApiErrorException(string errorCode)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="ex"></param>
        public ApiErrorException(string errorCode, Exception ex) : base(errorCode, ex)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        public ApiErrorException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
