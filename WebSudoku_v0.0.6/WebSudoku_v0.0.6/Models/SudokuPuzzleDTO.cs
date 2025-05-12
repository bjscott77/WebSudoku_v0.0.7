using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebSudoku_v0._0._6.Models
{
    [Table("Puzzles")]
    [Serializable]    
    public class SudokuPuzzleDTO
    {
        [JsonPropertyName("values")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Required]
        [Column("Values")]
        [StringLength(81, MinimumLength = 81, ErrorMessage = "Values must be exactly 81 characters long.")]
        [RegularExpression(@"^[1-9\.]{81}$", ErrorMessage = "Values must contain only digits 1-9 and ' ' for empty cells.")]
        public string[]? Values { get; set; } = [string.Empty];

        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive integer.")]
        public Guid? Id { get; set; }

        [JsonPropertyName("difficulty")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Required]
        [Column("Difficulty")]
        [MaxLength(10, ErrorMessage = "Difficulty must be 10 characters or less.")]
        public string? Difficulty { get; set; } = string.Empty;

        public string? Status { get; set; } = string.Empty;
        public int? StatusCode { get; set; }

        public SudokuPuzzleDTO(string[] values, Guid id, string difficulty, string status, int statusCode)
        {
            Values = values;
            Id = id;
            Difficulty = difficulty;
            Status = status;
            StatusCode = statusCode;
        }
    }
}

