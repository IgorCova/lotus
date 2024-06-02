using LotusWebApp.Data.Entities;
using LotusWebApp.Data.Models;
using LotusWebApp.Data.Models.Saga;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LotusWebApp.Data;

public class ApplicationDbContext : IdentityUserContext<ApplicationUser>, IContextRegistration
{
   private readonly DbContextOptions<ApplicationDbContext> _options;
   public DbSet<Page> Pages => Set<Page>();
   public DbSet<Billing> Billings => Set<Billing>();
   public DbSet<BillingHistory> BillingHistories => Set<BillingHistory>();
   public DbSet<Notification> Notifications => Set<Notification>();
   public DbSet<Order> Orders => Set<Order>();

   public DbSet<Customer> Customers => Set<Customer>();
   public DbSet<OrderRequestSagaInstance> OrderState => Set<OrderRequestSagaInstance>();

   public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
   {
       _options = options;
       AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
       AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
   }
   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(assembly: typeof(ApplicationDbContext).Assembly);
    }


    public void RegisterContext(IServiceCollection collection, IConfiguration configuration)
    {
        var dbCovashopConnectionString = configuration
            .GetConnectionString("db.lotus") ?? "Host=localhost;Port=7432;Database=lotus;Username=lotus;Password=lotus";

        collection.AddDbContext<ApplicationDbContext>(b =>
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

    public ApplicationDbContext GetNewInstance()
    {
        return new ApplicationDbContext(_options);
    }
}