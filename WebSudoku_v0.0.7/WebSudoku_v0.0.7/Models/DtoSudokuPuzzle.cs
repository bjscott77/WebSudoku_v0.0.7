
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSudoku_v0._0._7.Models
{
    [JsonSerializable(typeof(DtoSudokuPuzzle))]
    public class DtoSudokuPuzzle
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        [StringLength(36, ErrorMessage = "ID must be 36 characters long.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "ID must be a valid GUID.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public Guid Id { get; internal set; }

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
        public DtoSudokuPuzzle()
        {
            this.Id = Guid.NewGuid();
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
        }
        public DtoSudokuPuzzle(Guid id)
        {
            this.Id = id;
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
        }
        public DtoSudokuPuzzle(string id)
        {
            this.Id = Guid.Parse(id);
            Difficulty = int.MinValue;
            BoardValues = string.Empty;
        }
        public DtoSudokuPuzzle(string id, int difficulty)
        {
            this.Id = Guid.Parse(id);
            Difficulty = difficulty;
            BoardValues = string.Empty;
        }
        public DtoSudokuPuzzle(Guid id, int difficulty)
        {
            this.Id = id;
            Difficulty = difficulty;
            BoardValues = string.Empty;
        }
        public DtoSudokuPuzzle(string id, int difficulty, string boardValues)
        {
            this.Id = Guid.Parse(id);
            Difficulty = difficulty;
            BoardValues = boardValues;
        }
        public DtoSudokuPuzzle(Guid id, int difficulty, string boardValues)
        {
            this.Id = id;
            Difficulty = difficulty;
            BoardValues = boardValues;
        }
        #endregion

        //  GetIdByString() available from public EntityBase.GetIdByString()
    }
}
