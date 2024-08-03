using MediatR;
using Microsoft.AspNetCore.Mvc;
using TempConsumer.CodeThatShouldBeGenerated;

namespace TempConsumer.Features;

public record Student(string Name);

public record CreateStudentCommand(string Name) : IRequest<Student>;

[PostEndpoint(Route = "students")] //Fails if '/' is there 
public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Student>
{
    [HttpGet]
    public Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;

        return Task.FromResult(new Student(name));
    }
}
