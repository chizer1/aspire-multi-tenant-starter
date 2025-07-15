using Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Db;

public class ClientDbContext(DbContextOptions<ClientDbContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Products { get; set; }
}
