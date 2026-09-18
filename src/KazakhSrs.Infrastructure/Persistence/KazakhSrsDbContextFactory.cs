using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KazakhSrs.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations add` / `dotnet ef database update` run straight against this
/// class library, without a separate host/API project. Run from the Infrastructure folder:
///
///   dotnet ef migrations add InitialCreate --project src/KazakhSrs.Infrastructure --startup-project src/KazakhSrs.Infrastructure
///
/// Connection string comes from KAZAKHSRS_CONNECTION_STRING if set, otherwise a local-dev default.
/// </summary>
public class KazakhSrsDbContextFactory : IDesignTimeDbContextFactory<KazakhSrsDbContext>
{
    public KazakhSrsDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("KAZAKHSRS_CONNECTION_STRING")
            ?? "Host=localhost;Database=kazakhsrs;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<KazakhSrsDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new KazakhSrsDbContext(optionsBuilder.Options);
    }
}