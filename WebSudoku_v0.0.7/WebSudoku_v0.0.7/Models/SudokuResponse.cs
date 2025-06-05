namespace WebSudoku_v0._0._7.Models
{
    public class SudokuResponse : ISudokuResponse
    {
        public List<DTOSudoku> Payload { get; set; } = new List<DTOSudoku>();
        public int? StatusCode { get; set; } = 200;
        public string? Status { get; set; } = "OK";
        public string? ErrorMessage { get; set; } = string.Empty;
        public string? CellDisplayValueType { get; set; }
        public SudokuResponse()
        {
        }

        public SudokuResponse(int statusCode, string status, string errorMessage)
        {
            Payload = new List<DTOSudoku>();
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public SudokuResponse(List<DTOSudoku> model, int statusCode, string status, string errorMessage)
        {
            Payload = model;
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }
        public SudokuResponse(List<DTOSudoku> data)
        {
            Payload = data;
        }
    }
}
