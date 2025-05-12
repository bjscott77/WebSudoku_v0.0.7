namespace WebSudoku_v0._0._6.Models
{
    public interface ISudokuPuzzleResponseDTO
    {
        public string Values { get; set; }
        public string Difficulty { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
    }
}
