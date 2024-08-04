using MediatR;
using AutoApiGen.Attributes;

namespace TempConsumer.Features;

public record Student(string Name);

[PostEndpoint("students")] //Fails if '/' is there 
public record CreateStudentCommand(string Name) : IRequest<Student>;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Student>
{
    public Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;

        return Task.FromResult(new Student(name));
    }
}
