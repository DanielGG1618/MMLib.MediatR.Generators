## Given
```csharp
[PostEndpoint("/students")]
public record CreateStudentCommand(string Name) : ICommand<Student>;

public class CreateStudentHandler : ICommandHandler<CreateStudentCommand, Student>
{
    public async Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;
        
        return new Student(name);
    }
}

[GetEndpoint("/students/{Id}")]
public record GetStudentQuery(string Id) : IQuery<Student>;

public class GetStudentHandler : IQueryHandler<GetStudentQuery, Student>
{
    public async Task<Student> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var id = query.Id;
        
        return new Student(id);
    }
}
```

## Generates
```csharp
public class StudentsController(IMediator mediator) 
    : ApiController(mediator)
{
    [HttpPost("/students")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command)
    {
        var result = await Mediator.Send(command);
        
        return Created(result);
    }
    
    [HttpGet("/students/{id:string}")]
    public async Task<IActionResult> GetStudent(string id)
    {
        var result = await Mediator.Send(query);
        
        return Ok(result);
    }
}
```

## For Future Thought 
- Generating custom types in case of mix of route, query, and body parameters