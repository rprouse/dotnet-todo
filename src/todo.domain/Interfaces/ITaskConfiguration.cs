namespace Alteridem.Todo.Domain.Interfaces
{
    public interface ITaskConfiguration
    {
        string TaskDirectory { get; }

        string TodoFilename { get; }

        string DoneFilename { get; }
    }
}
