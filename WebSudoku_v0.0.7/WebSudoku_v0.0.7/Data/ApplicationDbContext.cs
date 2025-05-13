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

        public virtual DbSet<DtoSudokuPuzzle> Puzzle { get; set; } = null!; 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=SW_OFFICE\\VSDEVSERVER;Initial Catalog=Sudoku;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DtoSudokuPuzzle>()
                .HasKey(p => p.Id)
                .HasName("PK_SudokuPuzzle");

            modelBuilder.Entity<DtoSudokuPuzzle>()
                .Property(p => p.Difficulty)
                .IsRequired();

            modelBuilder.Entity<DtoSudokuPuzzle>()
                .Property(p => p.BoardValues)
                .IsRequired()
                .HasMaxLength(81);

            modelBuilder.Entity<DtoSudokuPuzzle>()
                .ToTable("SudokuPuzzles");
        }
    }
}
