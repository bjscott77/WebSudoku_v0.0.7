namespace WebSudoku_v0._0._6.Models
{
    public interface ISudokuPuzzleEntityModel
    {
        public string Values { get; set; }
        public string Difficulty { get; set; }
        public Guid Id { get; set; }    
    }
}
