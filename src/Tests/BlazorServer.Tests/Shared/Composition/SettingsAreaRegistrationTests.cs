using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;

namespace BlazorServer.Tests.Shared.Composition;

[TestFixture]
public class SettingsAreaRegistrationTests
{
    [Test]
    public void ModuleCanRegisterSettingsArea()
    {
        var context = new AppRegistrationContext();

        context.UiComposition.AddSettingsArea(
            new UiSettingsArea(
                "bidcon",
                "Bidcon",
                "Configure Bidcon integration.",
                "/settings/bidcon",
                20,
                "settings",
                "CanManageBidcon"),
            ModuleKey<TestModule>());

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

        RegisterArea<FirstModule>(context, "second", "/settings/second", 20);
        RegisterArea<SecondModule>(context, "first", "/settings/first", 10);
        RegisterArea<ThirdModule>(context, "third", "/settings/third", 20);

        Assert.That(
            context.UiComposition.SettingsAreas.Select(area => area.Id),
            Is.EqualTo(new[] { "first", "second", "third" }));
    }

    [Test]
    public void DuplicateSettingsAreaIdsFailRegistration()
    {
        var context = new AppRegistrationContext();
        RegisterArea<FirstModule>(context, "bidcon", "/settings/bidcon", 10);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RegisterArea<SecondModule>(context, "bidcon", "/settings/other", 20));

        Assert.That(exception!.Message, Does.Contain("bidcon").And.Contain("SettingsAreas"));
    }

    [Test]
    public void SecondSettingsAreaForSameModuleFailsRegistration()
    {
        var context = new AppRegistrationContext();
        RegisterArea<TestModule>(context, "bidcon", "/settings/bidcon", 10);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RegisterArea<TestModule>(context, "bidcon-import", "/settings/bidcon/import", 20));

        Assert.That(exception!.Message, Does.Contain(ModuleKey<TestModule>()).And.Contain("already registered"));
    }

    [TestCase("/bidcon")]
    [TestCase("/settings")]
    [TestCase("settings/bidcon")]
    public void InvalidSettingsAreaHrefsFailRegistration(string href)
    {
        var context = new AppRegistrationContext();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            RegisterArea<TestModule>(context, "bidcon", href, 10));

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

    private static void RegisterArea<TModule>(
        AppRegistrationContext context,
        string id,
        string href,
        int order)
    {
        context.UiComposition.AddSettingsArea(
            new UiSettingsArea(
                id,
                id,
                $"{id} settings",
                href,
                order),
            ModuleKey<TModule>());
    }

    private static string ModuleKey<TModule>()
    {
        return typeof(TModule).FullName ?? typeof(TModule).Name;
    }

    private sealed class TestModule;

    private sealed class FirstModule;

    private sealed class SecondModule;

    private sealed class ThirdModule;
}
