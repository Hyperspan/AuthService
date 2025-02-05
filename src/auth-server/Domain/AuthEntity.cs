using System;

namespace AuthServer.Domain
{
    public class AuthEntity<TId> where TId : <TId>
    {
        public TId Id { get; set; }
    }
}