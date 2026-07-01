using BlazorServer.Modules.Projects;
using BlazorServer.Modules.Projects.Features.CreateProject;
using BlazorServer.Shared.Composition;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServer.Tests.Modules.Projects;

[TestFixture]
public class ProjectsModuleRegistrationTests
{
    [Test]
    public void ServicesCanRegisterProjectsModule()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddProjectsModule(registrationContext);

        using var provider = services.BuildServiceProvider();

        Assert.Multiple(() =>
        {
            Assert.That(provider.GetRequiredService<IProjectStore>(), Is.Not.Null);
            Assert.That(provider.GetRequiredService<CreateProjectCommandHandler>(), Is.Not.Null);
        });
    }
}
