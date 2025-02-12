namespace AuthServer.Shared.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginResult : OperationResult
    {
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new static LoginResult Success()
        {
            return new LoginResult
            {
                Succeeded = true
            };
        }

    }
}
