using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedWarehousingCore.Model.DomainModel;
using SharedWarehousingCore.Model.IdentityModel;

namespace SharedWarehousingCore.DAL;

public class MainDbContext : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    //dotnet ef migrations add Init --project DataAccess --startup-project Api
    //dotnet ef database update --project DataAccess --startup-project Api
    //Rider/Datagrip db url:   jdbc:sqlserver://localhost:1433;database=FairTradeDb
    public MainDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<RecordHistory> RecordHistory { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new RecordHistoryMap());
    }

    public static void Seed(DbContext context)
    {
        var role = new AppRole { Name = "User" };

        if (!context.Set<AppRole>().Any(p => p.Name == role.Name))
        {
            context.Set<AppRole>().Add(role);
            context.SaveChanges();
        }
    }
    public async Task<int> SaveChangesWithLogsAsync(int userId,
        CancellationToken cancellationToken = new CancellationToken())
    {
        // var userName = _httpContextAccessor.HttpContext.User.Claims
        //     .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
        //     ?.Value ?? string.Empty;

        //
        // foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        // {
        //     switch (entry.State)
        //     {
        //         case EntityState.Added:
        //             entry.Entity.CreatedBy = userId;
        //             entry.Entity.CreatedDateTime = DateTime.UtcNow;
        //             break;
        //
        //         case EntityState.Modified:
        //             entry.Entity.UpdatedBy = userId;
        //             entry.Entity.UpdatedDateTime = DateTime.UtcNow;
        //             break;
        //     }
        // }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}