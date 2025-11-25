using Core.Models;
using Core.Models.Authorization;
using Core.Models.EstimationEnteties;
using Core.Models.EstimationView;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.EfCore;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    private readonly ValueComparer<ICollection<string>> _stringCollectionValueComparer = new((c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

    public required DbSet<BidconAccessCredentials> BidconAccessCredentials { get; set; }
    public required DbSet<View> Views { get; set; }
    public required DbSet<UserView> UserViews { get; set; }
    public required DbSet<RoleView> RoleViews { get; set; }
    public required DbSet<UserSettings> UserSettings { get; set; }
    public required DbSet<Role> Roles { get; set; }
    public required DbSet<RoleAccess> RoleAccess { get; set; }
    public required DbSet<UserRole> UserRoles { get; set; }
    public required DbSet<UserAccess> UserAccess { get; set; }
    public required DbSet<SdkCredentials> SdkCredentials { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RoleAccess>()
            .HasKey(ra => new { ra.RoleId, ra.AccessClaimValue });
        modelBuilder.Entity<UserAccess>()
            .HasKey(ua => new { ua.UserId, ua.AccessClaimValue });
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<RoleView>()
            .HasKey(rv => new { rv.RoleId, rv.ViewId });
        modelBuilder.Entity<UserView>()
            .HasKey(uv => new { uv.UserId, uv.ViewId });


        //modelBuilder.Entity<Estimation>()
        //    .HasOne(e => e.NetSheet)
        //    .WithOne(si => si.Estimation)
        //    .HasForeignKey<Estimation>(e => e.NetSheetId)
        //    .OnDelete(DeleteBehavior.Cascade);
        //modelBuilder.Entity<SheetItem>()
        //    .HasMany(si => si.Children)
        //    .WithOne(si => si.Parent)
        //    .HasForeignKey(si => si.ParentId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<SheetCellFormat>()
        //    .HasKey(sc => new { sc.SheetSectionId, sc.ColumnType, sc.RowType});
    }
}
