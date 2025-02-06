namespace AuthServer.Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class AuthEntity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public TId Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}