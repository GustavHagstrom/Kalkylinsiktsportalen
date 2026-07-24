using System.Security.Claims;
using BlazorServer.Modules.Access.Public.Authorization;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class AccessAuthorizationOwnershipTests
{
    [Test]
    public void AccessOwnsApplicationAdminDecisionContract()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(AccessAuthorizationConstants.AccessClaimType, "CanDoThing"),
            new Claim("test-admin", "true")
        ], "test"));
        IApplicationAdminPrincipalEvaluator evaluator = new ClaimTypeApplicationAdminPrincipalEvaluator("test-admin");

        Assert.Multiple(() =>
        {
            Assert.That(principal.HasClaim(AccessAuthorizationConstants.AccessClaimType, "CanDoThing"), Is.True);
            Assert.That(evaluator.IsApplicationAdmin(principal), Is.True);
        });
    }

    [Test]
    public void AccessDoesNotOwnEntraApplicationAdminClaimDetails()
    {
        var productionRoot = FindProductionRoot();
        var accessFiles = Directory.GetFiles(
            Path.Combine(productionRoot, "Modules", "Access"),
            "*.*",
            SearchOption.AllDirectories)
            .Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var violations = accessFiles
            .Where(file =>
            {
                var text = File.ReadAllText(file);
                return text.Contains("wids", StringComparison.Ordinal)
                    || text.Contains("62e90394-69f5-4237-9190-012177145e10", StringComparison.Ordinal);
            })
            .Select(file => Path.GetRelativePath(
                productionRoot,
                file))
            .ToArray();

        Assert.That(violations, Is.Empty,
            "Entra application-admin claim details belong in the provider adapter, not Modules/Access. Violations:\n"
            + string.Join('\n', violations));
    }

    private sealed class ClaimTypeApplicationAdminPrincipalEvaluator(string claimType) : IApplicationAdminPrincipalEvaluator
    {
        public bool IsApplicationAdmin(ClaimsPrincipal principal)
            => principal.HasClaim(claimType, "true");
    }

    private static string FindProductionRoot()
    {
        for (var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "src", "BlazorServer");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Could not locate src/BlazorServer from the test output directory.");
    }
}
