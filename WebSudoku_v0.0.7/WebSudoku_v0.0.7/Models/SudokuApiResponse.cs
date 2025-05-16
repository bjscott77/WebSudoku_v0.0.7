namespace WebSudoku_v0._0._7.Models
{
    public class SudokuApiResponse : ISudokuApiResponse
    {
        public List<SudokuPuzzledto> Payload { get; set; } = new List<SudokuPuzzledto>();
        public int StatusCode { get; set; } = 200;
        public string Status { get; set; } = "OK";
        public string ErrorMessage { get; set; } = string.Empty;
        public SudokuApiResponse()
        {
        }

        public SudokuApiResponse(int statusCode, string status, string errorMessage)
        {
            Payload = new List<SudokuPuzzledto>();
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public SudokuApiResponse(List<SudokuPuzzledto> model, int statusCode, string status, string errorMessage)
        {
            Payload = model;
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }
        public SudokuApiResponse(List<SudokuPuzzledto> data)
        {
            Payload = data;
        }
    }
}
