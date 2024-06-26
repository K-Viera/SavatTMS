using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TMSApi;

namespace TMSTests;

public class TestFixture
{
    public IServiceProvider ServiceProvider { get; private set; }
    public TestFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer("Server=MSI;Database=TmsDb;User Id=sa;Password=admin123;TrustServerCertificate=True;");
        });
        serviceCollection.AddScoped<IShipmentService, ShipmentService>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
    
    [CollectionDefinition("Shipments collection")]
    public class TestCollection : ICollectionFixture<TestFixture> { }
}
