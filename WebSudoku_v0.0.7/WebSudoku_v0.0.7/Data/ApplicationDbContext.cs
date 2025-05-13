using Microsoft.EntityFrameworkCore;
using WebSudoku_v0._0._7.Models;

namespace WebSudoku_v0._0._7.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(string connectionString) : base(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connectionString).Options)
        {
        }

        public virtual DbSet<SudokuPuzzledto> Puzzle { get; set; } = null!; 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=SW_OFFICE\\VSDEVSERVER;Initial Catalog=Sudoku;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SudokuPuzzledto>()
                .HasKey(p => p.Id)
                .HasName("PK_SudokuPuzzle");

            modelBuilder.Entity<SudokuPuzzledto>()
                .Property(p => p.Difficulty)
                .IsRequired();

            modelBuilder.Entity<SudokuPuzzledto>()
                .Property(p => p.BoardValues)
                .IsRequired()
                .HasMaxLength(81);

            modelBuilder.Entity<SudokuPuzzledto>()
                .ToTable("SudokuPuzzles");
        }
    }
}
