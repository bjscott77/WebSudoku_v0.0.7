namespace WebSudoku_v0._0._7.Models
{
    public abstract class EntityBase : IDisposable
    {
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
        public EntityBase(EntityBase entityBase)
        {
            this.Id = entityBase.Id;
        }

        public string GetIdAsString()
        {
            return this.Id.ToString();
        }

        public void Dispose()
        {
            this.Dispose();

        }
    }
}
