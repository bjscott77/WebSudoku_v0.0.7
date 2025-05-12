using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSudoku_v0._0._6.Models
{
    [JsonSerializable(typeof(SudokuPuzzleResponseDTO))]
    [JsonSourceGenerationOptions(WriteIndented = true)]
    public class SudokuPuzzleResponseDTO : ISudokuPuzzleResponseDTO
    {
        [JsonPropertyName("values")]
        [JsonInclude]
        [Required]
        public string Values { get; set; }

        [JsonPropertyName("id")]
        [JsonInclude]
        [Required]
        public Guid Id { get; set; }

        [JsonPropertyName("difficulty")]
        [JsonInclude]
        [Required]
        public string Difficulty { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
        public SudokuPuzzleResponseDTO()
        {
            Values = string.Empty;
            Difficulty = string.Empty;
            Status = string.Empty;
            StatusCode = 0;
        }
        public SudokuPuzzleResponseDTO(string values, string difficulty, string status, int statusCode)
        {
            Values = values;
            Difficulty = difficulty;
            Status = status;
            StatusCode = statusCode;
        }
    }
}
