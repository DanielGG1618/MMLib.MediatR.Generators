using System.ComponentModel.Design;
using MediatR;
using Microsoft.AspNetCore.Mvc;

Console.WriteLine(12);
var mediator = new Mediator(new ServiceContainer());
ControllerBase controller = new TempConsumer.StudentsController(mediator);
