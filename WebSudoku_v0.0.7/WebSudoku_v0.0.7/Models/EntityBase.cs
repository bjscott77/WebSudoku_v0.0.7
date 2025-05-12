using System.ComponentModel.DataAnnotations;

namespace WebSudoku_v0._0._7.Models
{
    public abstract class EntityBase
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        [StringLength(36, ErrorMessage = "ID must be 36 characters long.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "ID must be a valid GUID.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public Guid Id { get; internal set; }

        public EntityBase()
        {
            this.Id = Guid.NewGuid();
        }
        public EntityBase(Guid id)
        {
            this.Id = id;
        }
        public EntityBase(string id)
        {
            this.Id = Guid.Parse(id);
        }

        public string GetIdAsString()
        {
            return this.Id.ToString();
        }
    }
}
