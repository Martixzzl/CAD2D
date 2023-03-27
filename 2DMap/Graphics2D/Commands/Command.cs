using System.Threading.Tasks;

namespace Graphics2D
{
    /// <summary>
    /// Command基类
    /// </summary>
    public abstract class Command
    {
        public abstract string RegisteredName { get; }
        public abstract string Name { get; }

        public virtual Task Apply(Document doc, params string[] args)
        {
            return Task.FromResult(default(object));
        }
    }
}
