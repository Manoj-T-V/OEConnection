using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands.Handlers.Procedure;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;
using RL.Backend.Commands;
[TestClass]
public class DeleteUsersFromProcedureCommandHandlerTests
{
    private RLContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RLContext>()
            .UseInMemoryDatabase(databaseName: "Test_DeleteUsersFromProcedure")
            .Options;

        return new RLContext(options);
    }

    [TestMethod]
    public async Task Handle_InvalidPlanId_ReturnsBadRequest()
    {
        // Arrange
        var context = CreateContext();
        var sut = new DeleteUsersFromProcedureCommandHandler(context);

        var request = new DeleteUsersFromProcedureCommand
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
    public async Task Handle_ValidRequest_DeletesUsersSuccessfully()
    {
        // Arrange
        var context = CreateContext();

        var plan = new Plan { PlanId = 1 };
        var procedure = new Procedure { ProcedureId = 1 };
        var user = new User { UserId = 1 };

        var ppu = new PlanProcedureUser
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 1,
            Plan = plan,
            Procedure = procedure
        };

        context.Plans.Add(plan);
        context.Procedures.Add(procedure);
        context.Users.Add(user);
        context.PlanProcedureUsers.Add(ppu);
        await context.SaveChangesAsync();

        var sut = new DeleteUsersFromProcedureCommandHandler(context);

        var request = new DeleteUsersFromProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserIds = new List<int> { 1 }
        };

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();

        var deletedUser = context.PlanProcedureUsers.FirstOrDefault();
        deletedUser.Should().BeNull();
    }
}
