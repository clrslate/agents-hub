using ClrSlate.Modules.AgentsAppModule.Models;
using ClrSlate.Modules.AgentsAppModule.Validators;

namespace ClrSlate.Modules.AgentsAppModule.Tests.Validators;

[TestFixture]
public class CreateAgentRequestValidatorTests
{
    private CreateAgentRequestValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateAgentRequestValidator();

    [Test]
    public void Valid_Request_Passes()
    {
        var req = new CreateAgentRequest { Name = "support-bot", Model = new ModelReference { Name = "gpt-4o", Provider = "openai" } };
        var result = _validator.Validate(req);
        Assert.That(result.IsValid, Is.True, string.Join(";", result.Errors.Select(e => e.ErrorMessage)));
    }

    [Test]
    public void Invalid_Slug_Fails()
    {
        var req = new CreateAgentRequest { Name = "BadSlug", Model = new ModelReference { Name = "model", Provider = "openai" } };
        var result = _validator.Validate(req);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Instructions_Length_Boundary()
    {
        var okText = new string('a', 16000);
        var tooLong = new string('a', 16001);

        var okReq = new CreateAgentRequest { Name = "x", Model = new ModelReference { Name = "m", Provider = "test" }, Instructions = okText };
        var okResult = _validator.Validate(okReq);
        Assert.That(okResult.IsValid, Is.True, "16000 chars should be valid");

        var badReq = new CreateAgentRequest { Name = "x", Model = new ModelReference { Name = "m", Provider = "test" }, Instructions = tooLong };
        var badResult = _validator.Validate(badReq);
        Assert.That(badResult.IsValid, Is.False, "16001 chars should be invalid");
    }
}
