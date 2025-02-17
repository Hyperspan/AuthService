namespace AuthServer.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationPermission<TId> where TId : IEquatable<TId>
    {
        public TId Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
