using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;
using BlazorServer.Shared.ModuleRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServer.Tests.Shared.ModuleRegistrations;

[TestFixture]
public class SettingsAreaRegistrationTests
{
    [Test]
    public void ModuleCanRegisterSettingsArea()
    {
        var context = new AppRegistrationContext();
        var module = new ModuleRegistration(
            new ServiceCollection(),
            context,
            typeof(TestModule));

        module.Ui.Settings.SetArea(
            id: "bidcon",
            title: "Bidcon",
            description: "Configure Bidcon integration.",
            href: "/settings/bidcon",
            order: 20,
            icon: "settings",
            policy: "CanManageBidcon");

        Assert.That(context.UiComposition.SettingsAreas, Has.One.EqualTo(
            new UiSettingsArea(
                "bidcon",
                "Bidcon",
                "Configure Bidcon integration.",
                "/settings/bidcon",
                20,
                "settings",
                "CanManageBidcon")));
    }

    [Test]
    public void SettingsAreaDoesNotExposeInternalModuleKey()
    {
        Assert.That(
            typeof(UiSettingsArea).GetProperties().Select(property => property.Name),
            Does.Not.Contain("ModuleKey"));
    }

    [Test]
    public void SettingsAreasAreOrderedByOrderThenRegistrationOrder()
    {
        var context = new AppRegistrationContext();

        RegisterArea(context, typeof(FirstModule), "second", "/settings/second", 20);
        RegisterArea(context, typeof(SecondModule), "first", "/settings/first", 10);
        RegisterArea(context, typeof(ThirdModule), "third", "/settings/third", 20);

        Assert.That(
            context.UiComposition.SettingsAreas.Select(area => area.Id),
            Is.EqualTo(new[] { "first", "second", "third" }));
    }

    [Test]
    public void DuplicateSettingsAreaIdsFailRegistration()
    {
        var context = new AppRegistrationContext();
        RegisterArea(context, typeof(FirstModule), "bidcon", "/settings/bidcon", 10);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RegisterArea(context, typeof(SecondModule), "bidcon", "/settings/other", 20));

        Assert.That(exception!.Message, Does.Contain("bidcon").And.Contain("SettingsAreas"));
    }

    [Test]
    public void SecondSettingsAreaForSameModuleFailsRegistration()
    {
        var context = new AppRegistrationContext();
        RegisterArea(context, typeof(TestModule), "bidcon", "/settings/bidcon", 10);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RegisterArea(context, typeof(TestModule), "bidcon-import", "/settings/bidcon/import", 20));

        Assert.That(exception!.Message, Does.Contain(nameof(TestModule)).And.Contain("already registered"));
    }

    [TestCase("/bidcon")]
    [TestCase("/settings")]
    [TestCase("settings/bidcon")]
    public void InvalidSettingsAreaHrefsFailRegistration(string href)
    {
        var context = new AppRegistrationContext();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RegisterArea(context, typeof(TestModule), "bidcon", href, 10));

        Assert.That(exception!.Message, Does.Contain(href).And.Contain("/settings/"));
    }

    [Test]
    public void OldSettingsNavigationIsSeparateFromSettingsAreas()
    {
        var context = new AppRegistrationContext();

        context.RenderCollection.AddSettingsNavigation(
            text: "Legacy settings",
            href: "/legacy-settings",
            order: 10);

        Assert.Multiple(() =>
        {
            Assert.That(context.UiComposition.SettingsNavigation, Has.Count.EqualTo(1));
            Assert.That(context.UiComposition.SettingsAreas, Is.Empty);
        });
    }

    [Test]
    public void ExistingTopNavigationRegistrationStillWorks()
    {
        var context = new AppRegistrationContext();

        context.RenderCollection.AddTopNavigation(
            text: "Home",
            href: "/",
            order: 10);

        Assert.That(context.UiComposition.TopNavigation, Has.One.Matches<UiNavigationItem>(item =>
            item.Text == "Home" &&
            item.Href == "/" &&
            item.Order == 10));
    }

    private static void RegisterArea(
        AppRegistrationContext context,
        Type moduleType,
        string id,
        string href,
        int order)
    {
        new ModuleRegistration(new ServiceCollection(), context, moduleType)
            .Ui
            .Settings
            .SetArea(
                id: id,
                title: id,
                description: $"{id} settings",
                href: href,
                order: order);
    }

    private sealed class TestModule;

    private sealed class FirstModule;

    private sealed class SecondModule;

    private sealed class ThirdModule;
}
