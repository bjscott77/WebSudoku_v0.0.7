namespace WebSudoku_v0._0._7.Models
{
    public class SudokuResponse : ISudokuResponse
    {
        public List<SudokuDTO> Payload { get; set; } = new List<SudokuDTO>();
        public int? StatusCode { get; set; } = 200;
        public string? Status { get; set; } = "OK";
        public string? ErrorMessage { get; set; } = string.Empty;
        public string? CellDisplayValueType { get; set; }
        public string? PuzzleMode { get; set; }
        public bool? Solved { get; set; } = false;

        public SudokuResponse()
        {
        }

        public SudokuResponse(int statusCode, string status, string errorMessage)
        {
            Payload = new List<SudokuDTO>();
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public SudokuResponse(List<SudokuDTO> model, int statusCode, string status, string errorMessage)
        {
            Payload = model;
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }
        public SudokuResponse(List<SudokuDTO> data)
        {
            Payload = data;
        }
    }
}
