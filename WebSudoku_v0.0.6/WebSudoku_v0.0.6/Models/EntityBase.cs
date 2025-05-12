namespace WebSudoku_v0._0._6.Models
{
    public abstract class EntityBase
    {
        Guid Id { get; set; } = Guid.NewGuid();
    }
}
