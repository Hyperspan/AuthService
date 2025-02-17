namespace AuthServer.Shared.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ErrorDescription { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static OperationResult Success()
        {
            return new OperationResult
            {
                Succeeded = true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        /// <returns></returns>
        public static OperationResult Failed(string errorCode, string errorDescription)
        {
            return new OperationResult
            {
                ErrorCode = errorCode,
                ErrorDescription = errorDescription,
                Succeeded = false
            };
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class OperationResult<TData> : OperationResult
    {
        /// <summary>
        /// 
        /// </summary>
        public TData? Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static OperationResult<TData> Success(TData data)
        {
            return new OperationResult<TData>
            {
                Data = data,
                Succeeded = true
            };
        }
    }
}