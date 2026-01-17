using Microsoft.EntityFrameworkCore;

namespace ValenceHub.Persistence.Contexts;

public class ValenceHubDbContext : DbContext
{
    public ValenceHubDbContext(DbContextOptions<ValenceHubDbContext> options)
        : base(options) { }
}
