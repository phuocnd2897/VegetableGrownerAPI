using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace VG.Context
{
    public interface IRepository<T, K> where T : class
    {
        /// <summary>
        /// Thêm 1 thực thể
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns></returns>
        T Add(T entity);
        /// <summary>
        /// Thêm danh sách thực thể
        /// </summary>
        /// <param name="entities">Danh sách thực thể</param>
        void Add(IEnumerable<T> entities);
        /// <summary>
        /// Thay đổi 1 thực thể
        /// </summary>
        /// <param name="entity">Thực thể</param>
        void Update(T entity);
        /// <summary>
        /// Thay đổi danh sách thực thể
        /// </summary>
        /// <param name="entities">Danh sách thực thể</param>
        void Update(IEnumerable<T> entities);
        /// <summary>
        /// Xóa thực thể theo Khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        void Delete(K id);
        /// <summary>
        /// Xóa thực thể
        /// </summary>
        /// <param name="entity">Thực thể</param>
        void Delete(T entity);
        /// <summary>
        /// Xóa danh sách thực thể
        /// </summary>
        /// <param name="entities">Danh sách thực thể</param>
        void Delete(IEnumerable<T> entities);
        /// <summary>
        /// Xóa theo điều kiện
        /// </summary>
        /// <param name="where">Điều kiện</param>
        void Delete(Expression<Func<T, bool>> where);
        /// <summary>
        /// Lấy thực thể theo khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns>Thực thể</returns>
        T GetSingle(K id);
        /// <summary>
        /// Lấy thực thể đầu tiên theo điều kiện
        /// </summary>
        /// <param name="where">Điều kiện</param>
        /// <param name="includes">Danh sách liên quan</param>
        /// <param name="partionColumn">Cột được partion kiểu datetime</param>
        /// <param name="partionColumn">Chuổi filegroup</param>
        /// <returns>Thực thể</returns>
        T GetSingle(Expression<Func<T, bool>> where, string[] includes = null, Expression<Func<T, DateTime>> partionColumn = null, string strfilegroup = null);
        /// <summary>
        /// Lấy tất cả thực thể
        /// </summary>
        /// <param name="includes">Danh sách liên quan</param>
        /// <returns>Danh sách thực thể</returns>
        IEnumerable<T> GetAll(string[] includes = null);
        /// <summary>
        /// Lấy danh sách thực thể theo điều kiện
        /// </summary>
        /// <param name="where">Điều kiện</param>
        /// <param name="includes">Danh sách liên quan</param>
        /// <param name="partionColumn">Cột được partion kiểu datetime</param>
        /// <param name="partionColumn">Chuổi filegroup</param>
        /// <returns>Danh sách thực thể</returns>
        IEnumerable<T> GetMulti(Expression<Func<T, bool>> where, string[] includes = null, Expression<Func<T, DateTime>> partionColumn = null, string strfilegroup = null);
        /// <summary>
        /// Lấy danh sách thực thể theo điều kiện có phân trang
        /// </summary>
        /// <param name="where">Điều kiện</param>
        /// <param name="sort">Sắp xếp</param>
        /// <param name="total">out Tổng sô thực thể</param>
        /// <param name="index">trang thứ</param>
        /// <param name="size">số lượng thực thể trên 1 trang</param>
        /// <param name="includes">Danh sách liên quan</param>
        /// <returns>Danh sách thực thể</returns>
        IEnumerable<T> GetMultiPaging(Expression<Func<T, bool>> where, Expression<Func<T, string>> sort, out int total, int index = 0, int size = 50, string[] includes = null);
        /// <summary>
        /// Đếm số lượng thực thể theo điều kiện
        /// </summary>
        /// <param name="where">Điều kiện</param>
        /// <returns>Số lượng</returns>
        int Count(Expression<Func<T, bool>> where);
        /// <summary>
        /// Kiểm tra sự tồn tại của thực thể theo điều kiện
        /// </summary>
        /// <param name="where">Điều kiện</param>
        /// <returns>true: Có tồn tại, false: không tồn tại</returns>
        bool Contains(Expression<Func<T, bool>> where);
        /// <summary>
        /// Lấy giá trị lớn nhất của cột Stt
        /// </summary>
        int GetMaxStt(Expression<Func<T, int>> exp);
        DbSet<U> GetDbSet<U>() where U : class;
        IEnumerable<U> GetDbSet<U>(Expression<Func<U, bool>> where, string[] includes = null, Expression<Func<U, DateTime>> partionColumn = null, string strfilegroup = null) where U : class;
        IQueryable<T> AsNoTracking();
        void Commit();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
