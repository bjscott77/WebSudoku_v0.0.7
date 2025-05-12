using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebSudoku_v0._0._6.Models;

namespace WebSudoku_v0._0._6.Data
{
    public class AppDbContext(IConfiguration? _configuration) : DbContext, IDisposable   
    {
        public DbSet<SudokuPuzzleEntityModel> Puzzles { get; set; }
        public AppDbContext() : this(null) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(EntityBase)));
            foreach (var entityType in entityTypes)
            {
                var entityTypeBuilder = modelBuilder.Entity(entityType);
                var tableName = entityType.Name + "s"; // Pluralize the table name
                entityTypeBuilder.ToTable(tableName);
            }
            base.OnModelCreating(modelBuilder);
        }

        public override void Dispose()
        {
            base.Dispose();
            Dispose();
        }
    }
}
