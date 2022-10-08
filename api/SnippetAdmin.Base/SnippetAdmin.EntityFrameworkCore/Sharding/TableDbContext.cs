using Microsoft.EntityFrameworkCore;

namespace Hawthorn.EntityFramework.Sharding
{
    internal class TableDbContext : DbContext
    {
        private string _tableName;

        private Type _type;

        public TableDbContext(DbContextOptions<TableDbContext> options, string tableName, Type type)
            : base(options)
        {
            _tableName = tableName;
            _type = type;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity(_type).ToTable(_tableName);
        }
    }
}
