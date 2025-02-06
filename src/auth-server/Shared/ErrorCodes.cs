namespace AuthServer.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public static class ErrorCodes
    {

        #region 00 System
        /// <summary>
        /// 
        /// </summary>
        public const string UnknownSystemException = "00SYS001";
        /// <summary>
        /// 
        /// </summary>
        public const string NotImplemented = "00SYS002";
        /// <summary>
        /// 
        /// </summary>
        public const string ArgumentNull = "00SYS003";
        /// <summary>
        /// 
        /// </summary>
        public const string NullValue = "00SYS004";

        #endregion

        #region 01 Authentication
        /// <summary>
        /// 
        /// </summary>
        public const string EmailTaken = "01AU001";
        /// <summary>
        /// 
        /// </summary>
        public const string PasswordNotStrong = "01AU002";
        /// <summary>
        /// 
        /// </summary>
        public const string IncorrectCredentials = "01AU003";
        /// <summary>
        /// 
        /// </summary>
        public const string UserNotFound = "01AU004";
        /// <summary>
        /// 
        /// </summary>
        public const string EmailNotVerified = "01AU005";
        /// <summary>
        /// 
        /// </summary>
        public const string MobileNotVerified = "01AU006";
        /// <summary>
        /// 
        /// </summary>
        public const string RoleExists = "01AU007";
        /// <summary>
        /// 
        /// </summary>
        public const string IdentityError = "01AU008";

        /// <summary>
        /// 
        /// </summary>
        public const string InvalidEmailAddress = "01AU009";
        #endregion

        #region 02 Database

        /// <summary>
        /// 
        /// </summary>
        public const string DatabaseUnknownError = "02DB000";
        /// <summary>
        /// 
        /// </summary>
        public const string NullConnectionString = "02DB001";
        /// <summary>
        /// 
        /// </summary>
        public const string InsertFailed = "02DB002";
        /// <summary>
        /// 
        /// </summary>
        public const string UpdateFailed = "02DB003";
        /// <summary>
        /// 
        /// </summary>
        public const string DeleteFailed = "02DB004";
        /// <summary>
        /// 
        /// </summary>
        public const string QueryFailed = "02DB005";
        /// <summary>
        /// 
        /// </summary>
        public const string InvalidId = "02DB006";
        /// <summary>
        /// 
        /// </summary>
        public const string RecordNotFound = "02DB007";


        #endregion
    }
}
