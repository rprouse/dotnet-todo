using Alteridem.Todo.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Alteridem.Todo.Application.Commands
{
    public class EditTodoCommand : IRequest<Unit>
    {
        // No parameters for this command
    }

    public class EditTodoCommandHandler : IRequestHandler<EditTodoCommand, Unit>
    {
        private readonly ITaskConfiguration _configuration;

        public EditTodoCommandHandler(ITaskConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<Unit> Handle(EditTodoCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Get the directory that contains todo.txt from the configuration
            var todoDirectory = _configuration.TodoDirectory;

            // Step 2: Launch a process 'code' with the todoDirectory as a parameter
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "code",
                Arguments = $"\"{todoDirectory}\"",
                UseShellExecute = true,
                CreateNoWindow = true,
                WorkingDirectory = $"\"{todoDirectory}\"",
            };
            System.Diagnostics.Process.Start(processStartInfo);

            // Stub implementation for editing logic
            return Task.FromResult(Unit.Value);
        }
    }
}
