﻿
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSudoku_v0._0._7.Models
{
    public class SudokuPuzzledto : EntityBase
    {
        [Required]
        [Display(Name = "Difficulty")]
        [JsonPropertyName("difficulty")]
        [Range(1, 5)]
        [RegularExpression(@"^[1-5]$", ErrorMessage = "Difficulty must be a single digit between 1 and 5.")]
        public int Difficulty { get; set; }

        [Required]
        [Display(Name = "Board Values")]
        [JsonPropertyName("boardValues")]
        [StringLength(81, ErrorMessage = "Board values must be 81 characters long.")]
        [RegularExpression(@"^[0-9\]{81}$", ErrorMessage = "Board values must be a string of 81 characters, using digits 0-9.")]
        public string BoardValues { get; set; }

        #region Constructors
        public SudokuPuzzledto() : base()
        {
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
        }
        public SudokuPuzzledto(Guid id) : base(id)
        {
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
        }
        public SudokuPuzzledto(string id) : base(id)
        {
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
        }
        public SudokuPuzzledto(string id, int difficulty) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = string.Empty;
        }
        public SudokuPuzzledto(Guid id, int difficulty) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = string.Empty;
        }
        public SudokuPuzzledto(string id, int difficulty, string boardValues) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = boardValues;
        }
        public SudokuPuzzledto(Guid id, int difficulty, string boardValues) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = boardValues;
        }
        #endregion

        //  GetIdByString() available from public EntityBase.GetIdByString()
    }
}
