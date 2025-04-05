using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Presistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
{


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    #region DbSets
    public DbSet<Poll> Polls { get; set; }
    #endregion
}
