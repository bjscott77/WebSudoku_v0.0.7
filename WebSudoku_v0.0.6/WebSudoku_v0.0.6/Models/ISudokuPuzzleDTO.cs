namespace WebSudoku_v0._0._6.Models
{
    public interface ISudokuPuzzleDTO
    {
        public string Values { get; set; }
        public int MyProperty { get; set; }
        public string Difficulty { get; set; }
        public static string? Status { get; set; }
        public static int? StatusCode { get; set; }

    }
}
