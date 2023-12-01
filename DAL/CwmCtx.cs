using Microsis.CWM.AppService;
using Microsis.CWM.Model;
using Microsoft.EntityFrameworkCore;

namespace Microsis.CWM.DAL
{
    public interface ICwmCtx
    {

        DbSet<Wholesale> Wholesale { get; set; }
        DbSet<SaleInformation> salesInformation { get; set; }
        DbSet<MediaSettings> mediaSettings { get; set; }
        DbSet<Media> Media { get; set; }

        bool AddEntity<TEntity>(TEntity entity);
        bool AddEntityAsync<TEntity>(TEntity entity);
        void RemoveEntity<TEntity>(TEntity entity) where TEntity : class;
        int Rollback();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void UpdateEntity<TEntity>(TEntity entity) where TEntity : class;
    }

    public class CwmCtx : DbContext, IDisposable, ICwmCtx
    {

        public CwmCtx(DbContextOptions<CwmCtx> options) : base(options)
        {

        }

        #region ______ Main ______
        public DbSet<Wholesale> Wholesale { get; set; }
        public DbSet<SaleInformation> salesInformation { get; set; }
        public DbSet<MediaSettings> mediaSettings { get; set; }
        public DbSet<Media> Media { get; set; }

        #endregion

        //public DbSet<vwUserAccess> vwUserAccess { get; set; }


        #region ______ Methods ______
        public new int SaveChanges()
        {
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool AddEntity<TEntity>(TEntity entity)
        {
            base.Entry(entity).State = EntityState.Added;
            return true;
        }

        public bool AddEntityAsync<TEntity>(TEntity entity)
        {
            AddEntityAsync(entity);

            return true;
        }

        public int Rollback()
        {

            return 0;
        }

        public void UpdateEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void RemoveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        #endregion
    }
}
