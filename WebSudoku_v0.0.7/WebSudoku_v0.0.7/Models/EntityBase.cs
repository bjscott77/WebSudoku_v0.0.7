namespace WebSudoku_v0._0._7.Models
{
    public abstract class EntityBase : IDisposable
    {
        public Guid Id { get; set; }

        public void Dispose()
        {
            this.Dispose();

        }
    }
}
