using MediatR;
using AutoApiGen.Attributes;

namespace TempConsumer.Features;

public record GetStudentQuery(string Name) : IRequest<Student>;

[GetEndpoint("students")] //Fails if '/' is there 
public class GetStudentHandler : IRequestHandler<GetStudentQuery, Student>
{
    public Task<Student> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var name = query.Name;

        return Task.FromResult(new Student(name));
    }
}
