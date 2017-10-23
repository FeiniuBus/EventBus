using Microsoft.EntityFrameworkCore;

namespace EventBus.Core.Test
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }
}
