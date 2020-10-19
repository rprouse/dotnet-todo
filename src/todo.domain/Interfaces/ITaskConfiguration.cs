namespace Alteridem.Todo.Domain.Interfaces
{
    public interface ITaskConfiguration
    {
        string TaskDirectory { get; }

        string GetFullFilename(string filename);
    }
}
