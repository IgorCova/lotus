using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LotusWebApp.Data;

public class MainDbContext: DbContext, IContextRegistration
{
    private readonly DbContextOptions<MainDbContext> _options;
    public DbSet<User> Users { get; set; }

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
        _options = options;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(assembly: typeof(MainDbContext).Assembly);
    }

    public void RegisterContext(IServiceCollection collection, IConfiguration configuration)
    {
        var dbCovashopConnectionString = configuration
            .GetConnectionString("db.lotus") ?? "Host=localhost;Port=7432;Database=lotus;Username=lotus;Password=lotus";

        collection.AddDbContext<MainDbContext>(b =>
            {
                b.UseNpgsql(dbCovashopConnectionString, x =>
                {
                    x.EnableRetryOnFailure(3);
                    x.CommandTimeout(60);
                    x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                b.EnableDetailedErrors();
            }
        );
    }

    public MainDbContext GetNewInstance()
    {
        return new MainDbContext(_options);
    }
}