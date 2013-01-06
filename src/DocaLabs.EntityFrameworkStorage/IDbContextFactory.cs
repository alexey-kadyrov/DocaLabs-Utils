using System.Data.Entity;

namespace DocaLabs.EntityFrameworkStorage
{
    public interface IDbContextFactory
    {
        DbContext Create();
    }
}
