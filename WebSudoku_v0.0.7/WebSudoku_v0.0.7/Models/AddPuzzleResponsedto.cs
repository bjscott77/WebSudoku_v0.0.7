namespace WebSudoku_v0._0._7.Models
{
    public class AddPuzzleResponsedto : IAddPuzzleResponsedto
    {
        public List<SudokuPuzzledto> Payload { get; set; } = new List<SudokuPuzzledto>();
        public int StatusCode { get; set; } = 200;
        public string Status { get; set; } = "OK";
        public string ErrorMessage { get; set; } = string.Empty;
        public AddPuzzleResponsedto()
        {
        }

        public AddPuzzleResponsedto(int statusCode, string status, string errorMessage)
        {
            Payload = new List<SudokuPuzzledto>();
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public AddPuzzleResponsedto(List<SudokuPuzzledto> model, int statusCode, string status, string errorMessage)
        {
            Payload = model;
            StatusCode = statusCode;
            Status = status;
            ErrorMessage = errorMessage;
        }
        public AddPuzzleResponsedto(List<SudokuPuzzledto> data)
        {
            Payload = data;
        }
    }
}
