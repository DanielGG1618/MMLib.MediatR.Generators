using System.ComponentModel.Design;
using MediatR;
using TempConsumer;
using Microsoft.AspNetCore.Mvc;

Console.WriteLine(12);
var mediator = new Mediator(new ServiceContainer());
ControllerBase controller = new StudentsController(mediator);
controller = new ContestsController(mediator);
