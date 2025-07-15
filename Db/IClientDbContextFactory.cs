using Microsoft.EntityFrameworkCore;

namespace Db;

public interface IClientDbContextFactory
{
    Task<ClientDbContext> CreateDbContextAsync();
}
