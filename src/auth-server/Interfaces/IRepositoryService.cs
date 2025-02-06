using Microsoft.EntityFrameworkCore;

namespace AuthServer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRepository<in TId, T, out TContext>
        where TId : IEquatable<TId>
        where T : class
        where TContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        TContext Context { get; }
        /// <summary>
        /// 
        /// </summary>
        IQueryable<T> Entities { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> GetCount();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        Task<int> GetCount(string sqlQuery);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetById(TId id);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(string sqlQuery);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddRangeAsync(List<T> entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity);
    }
}
