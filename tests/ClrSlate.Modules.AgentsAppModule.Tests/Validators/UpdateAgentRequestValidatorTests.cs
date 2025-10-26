using ClrSlate.Modules.AgentsAppModule.Models;
using ClrSlate.Modules.AgentsAppModule.Validators;

namespace ClrSlate.Modules.AgentsAppModule.Tests.Validators;

[TestFixture]
public class UpdateAgentRequestValidatorTests
{
    private UpdateAgentRequestValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new UpdateAgentRequestValidator();

    [Test]
    public void Valid_Update_Passes()
    {
        var req = new UpdateAgentRequest { Model = "openai:gpt-4o", Version = 1 };
        var result = _validator.Validate(req);
        Assert.That(result.IsValid, Is.True, string.Join(";", result.Errors.Select(e => e.ErrorMessage)));
    }

    [Test]
    public void Version_Must_Be_Positive()
    {
        var req = new UpdateAgentRequest { Model = "m", Version = 0 };
        var result = _validator.Validate(req);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Instructions_Length_Boundary()
    {
        var okText = new string('a', 16000);
        var tooLong = new string('a', 16001);

        var okReq = new UpdateAgentRequest { Model = "m", Version = 1, Instructions = okText };
        var okResult = _validator.Validate(okReq);
        Assert.That(okResult.IsValid, Is.True);

        var badReq = new UpdateAgentRequest { Model = "m", Version = 1, Instructions = tooLong };
        var badResult = _validator.Validate(badReq);
        Assert.That(badResult.IsValid, Is.False);
    }
}
