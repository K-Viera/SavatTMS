using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory; // Add this using directive
using Moq;
using TMSApi;

namespace TMSTests;

public class ShipmentServiceTests
{
    private readonly IShipmentService _shipmentService;
    private readonly ApplicationDbContext _dbContext;

    public ShipmentServiceTests()
    {
        // Create a new instance of the ShipmentService class
        _dbContext = CreateDbContext("ShipmentServiceTests");
        _shipmentService = new ShipmentService(_dbContext);
    }
    private ApplicationDbContext CreateDbContext(string databaseName)
    {
        // Configure the DbContextOptions to use an in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Create an instance of the ApplicationDbContext
        var dbContext = new ApplicationDbContext(options);

        return dbContext;
    }

#region Add
    [Fact]
    public async Task AddShipmentAsync_ShouldAddShipment_WhenShipmentIsValid()
    {
        // Arrange
        var newShipment = new ShipmentDTO("1", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        // Act
        await _shipmentService.AddAsync(newShipment);

        // Assert
        var shipment = _dbContext.Shipments.FirstOrDefault(s => s.TrackingNumber == "1");
        Assert.NotNull(shipment);
    }

    [Fact]
    public async Task AddShipmentAsync_ShouldThrowException_WhenShipmentAlreadyExists()
    {
        // Arrange
        var existingShipment = new ShipmentDTO("2", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await _shipmentService.AddAsync(existingShipment);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _shipmentService.AddAsync(existingShipment));
    }
#endregion

#region Update
    [Fact]
    public async Task UpdateShipmentAsync_ShouldUpdateShipment_WhenShipmentExists()
    {
        // Arrange
        var existingShipment = new ShipmentDTO("3", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await _shipmentService.AddAsync(existingShipment);

        var updatedShipment = new ShipmentDTO("3", "Delivered", "New York", "Los Angeles", 100, DateTime.Now, DateTime.Now);

        // Act
        await _shipmentService.UpdateAsync(updatedShipment);

        // Assert
        var shipment = await _shipmentService.GetByTrackingNumberAsync("3");
        Assert.Equal("Delivered", shipment?.Status);
    }

    [Fact]
    public async Task UpdateShipmentAsync_ShouldThrowException_WhenShipmentDoesNotExist()
    {
        // Arrange
        var shipment = new ShipmentDTO("4", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _shipmentService.UpdateAsync(shipment));
    }
#endregion

#region Delete
    [Fact]
    public async Task DeleteShipmentAsync_ShouldDeleteShipment_WhenShipmentExists()
    {
        // Arrange
        var shipment = new ShipmentDTO("5", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await _shipmentService.AddAsync(shipment);

        // Act
        await _shipmentService.DeleteByTrackingNumberAsync("5");

        // Assert
        var deletedShipment = await _shipmentService.GetByTrackingNumberAsync("5");
        Assert.Null(deletedShipment);
    }

    [Fact]
    public async Task DeleteShipmentAsync_ShouldThrowException_WhenShipmentDoesNotExist()
    {

        // Arrange
        var shipment = new ShipmentDTO("6", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await _shipmentService.AddAsync(shipment);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _shipmentService.DeleteByTrackingNumberAsync("9999"));
    }
#endregion

#region GetByTrackingNumber
    [Fact]
    public async Task GetByTrackingNumberAsync_ShouldReturnShipment_WhenShipmentExists()
    {
        // Arrange
        var shipment = new ShipmentDTO("6", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await _shipmentService.AddAsync(shipment);

        // Act
        var result = await _shipmentService.GetByTrackingNumberAsync("6");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("6", result?.TrackingNumber);
    }

    [Fact]
    public async Task GetByTrackingNumberAsync_ShouldReturnNull_WhenShipmentDoesNotExist()
    {
        // Arrange
        var shipment = new ShipmentDTO("7", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await _shipmentService.AddAsync(shipment);

        // Act
        var result = await _shipmentService.GetByTrackingNumberAsync("8");

        // Assert
        Assert.Null(result);
    }
#endregion

#region GetAll
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllShipments()
    {
        var newDbContext = CreateDbContext("GetAllAsync_ShouldReturnAllShipments");
        var shipmentService = new ShipmentService(newDbContext);
        // Arrange
        var shipment1 = new ShipmentDTO("9", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        var shipment2 = new ShipmentDTO("10", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await shipmentService.AddAsync(shipment1);
        await shipmentService.AddAsync(shipment2);

        // Act
        var result = await shipmentService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoShipmentsExist()
    {
        var newDbContext = CreateDbContext("GetAllAsync_ShouldReturnEmpty_WhenNoShipmentsExist");
        var shipmentService = new ShipmentService(newDbContext);
        // Act
        var result = await shipmentService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
#endregion

#region Search
// public async Task<IEnumerable<ShipmentDTO>> SearchAsync(decimal? weight, DateTime? shippingDate, DateTime? deliveryDate, string? origin, string? destination, string? status)
    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingShipments()
    {
        var newDbContext = CreateDbContext("SearchAsync_ShouldReturnMatchingShipments");
        var shipmentService = new ShipmentService(newDbContext);
        // Arrange
        var shipment1 = new ShipmentDTO("11", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        var shipment2 = new ShipmentDTO("12", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await shipmentService.AddAsync(shipment1);
        await shipmentService.AddAsync(shipment2);

        // Act
        var result = await shipmentService.SearchAsync(null, null, null, null, null, "In Transit");

        // Assert
        //should return 2
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnEmpty_WhenNoMatchingShipmentsExist()
    {
        var newDbContext = CreateDbContext("SearchAsync_ShouldReturnEmpty_WhenNoMatchingShipmentsExist");
        var shipmentService = new ShipmentService(newDbContext);
        // Arrange
        var shipment1 = new ShipmentDTO("13", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        var shipment2 = new ShipmentDTO("14", "In Transit", "New York", "Los Angeles", 100, DateTime.Now, null);
        await shipmentService.AddAsync(shipment1);
        await shipmentService.AddAsync(shipment2);

        // Act
        var result = await shipmentService.SearchAsync(null, null, null, null, null, "Delivered");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
#endregion

}