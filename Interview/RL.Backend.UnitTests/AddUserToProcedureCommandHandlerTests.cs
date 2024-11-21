using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands.Handlers.Procedure;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RL.Backend.Commands;


[TestClass]
public class AddUserToProcedureCommandHandlerTests
{
    private RLContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RLContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddUserToProcedure")
            .Options;

        return new RLContext(options);
    }

    [TestMethod]
    public async Task Handle_InvalidPlanId_ReturnsBadRequest()
    {
        // Arrange
        var context = CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);

        var request = new AddUserToProcedureCommand
        {
            PlanId = 0,
            ProcedureId = 1,
            UserIds = new List<int> { 1 }
        };

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
        //result.Exception.Message.Should().Contain("Invalid PlanId");
    }

    [TestMethod]
    public async Task Handle_InvalidProcedureId_ReturnsBadRequest()
    {
        // Arrange
        var context = CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);

        var request = new AddUserToProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 0,
            UserIds = new List<int> { 1 }
        };

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
        //result.Exception.Message.Should().Contain("Invalid ProcedureId");
    }

    [TestMethod]
    public async Task Handle_ValidRequest_AddsUsersSuccessfully()
    {
        // Arrange
        var context = CreateContext();

        var plan = new Plan { PlanId = 1 };
        var procedure = new Procedure { ProcedureId = 1 };
        var user = new User { UserId = 1 };

        context.Plans.Add(plan);
        context.Procedures.Add(procedure);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var sut = new AddUserToProcedureCommandHandler(context);

        var request = new AddUserToProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserIds = new List<int> { 1 }
        };

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();

        var addedUser = context.PlanProcedureUsers.FirstOrDefault();
        addedUser.Should().NotBeNull();
        addedUser?.PlanId.Should().Be(1);
        addedUser?.ProcedureId.Should().Be(1);
        addedUser?.UserId.Should().Be(1);
    }
}
