using System.ComponentModel.Design;
using MediatR;
using Microsoft.AspNetCore.Mvc;

Console.WriteLine(1);
ControllerBase a = new TempConsumer.UsersController(new Mediator(new ServiceContainer()));
