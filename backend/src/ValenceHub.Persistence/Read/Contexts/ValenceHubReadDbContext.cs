using Microsoft.EntityFrameworkCore;

namespace ValenceHub.Persistence.Read.Contexts;

public class ValenceHubReadDbContext : DbContext
{
    public ValenceHubReadDbContext(DbContextOptions<ValenceHubReadDbContext> options)
        : base(options) { }
}
