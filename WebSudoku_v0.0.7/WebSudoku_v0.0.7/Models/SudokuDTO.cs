
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSudoku_v0._0._7.Models
{
    public class SudokuDTO : EntityBase
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
        [RegularExpression(@"^[0-9]{81}$", ErrorMessage = "Board values must be a string of 81 characters, using digits 0-9.")]
        public string BoardValues { get; set; }

        [Required]
        [Display(Name = "Possibles")]
        [JsonPropertyName("possibles")]
        public IEnumerable<string> Possibles { get; set; }

        #region Constructors
        public SudokuDTO() : base()
        {
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
            Possibles = new List<string>();
        }
        public SudokuDTO(Guid id) : base(id)
        {
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
            Possibles = new List<string>();
        }
        public SudokuDTO(string id) : base(id)
        {
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
            Possibles = new List<string>();
        }
        public SudokuDTO(string id, int difficulty) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = string.Empty;
            Possibles = new List<string>();
        }
        public SudokuDTO(Guid id, int difficulty) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = string.Empty;
            Possibles = new List<string>();
        }
        public SudokuDTO(string id, int difficulty, string boardValues) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = boardValues;
            Possibles = new List<string>();
        }
        public SudokuDTO(Guid id, int difficulty, string boardValues) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = boardValues;
            Possibles = new List<string>();
        }
        public SudokuDTO(string id, int difficulty, string boardValues, IEnumerable<string> possibles) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = boardValues;
            Possibles = possibles;
        }
        public SudokuDTO(Guid id, int difficulty, string boardValues, IEnumerable<string> possibles) : base(id)
        {
            Difficulty = difficulty;
            BoardValues = boardValues;
            Possibles = possibles;
        }
        #endregion

        //  GetIdByString() available from public EntityBase.GetIdByString()
    }
}
