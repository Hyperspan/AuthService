namespace AuthServer.Shared.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Succeeded { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IncorrectCredentials { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsTwoFactorRequired { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool EmailNotConfirmed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool PhoneNotConfirmed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNotActive { get; set; }
    }
}
