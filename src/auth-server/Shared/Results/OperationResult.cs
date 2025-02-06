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
    }
}
