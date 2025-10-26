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
        var req = new CreateAgentRequest { Name = "support-bot", Model = "openai:gpt-4o" };
        var result = _validator.Validate(req);
        Assert.That(result.IsValid, Is.True, string.Join(";", result.Errors.Select(e => e.ErrorMessage)));
    }

    [Test]
    public void Invalid_Slug_Fails()
    {
        var req = new CreateAgentRequest { Name = "BadSlug", Model = "model" };
        var result = _validator.Validate(req);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Instructions_Length_Boundary()
    {
        var okText = new string('a', 16000);
        var tooLong = new string('a', 16001);

        var okReq = new CreateAgentRequest { Name = "x", Model = "m", Instructions = okText };
        var okResult = _validator.Validate(okReq);
        Assert.That(okResult.IsValid, Is.True, "16000 chars should be valid");

        var badReq = new CreateAgentRequest { Name = "x", Model = "m", Instructions = tooLong };
        var badResult = _validator.Validate(badReq);
        Assert.That(badResult.IsValid, Is.False, "16001 chars should be invalid");
    }
}
