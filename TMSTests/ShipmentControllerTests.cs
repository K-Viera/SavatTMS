using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TMSApi;

namespace TMSTests;

public class ShipmentControllerTests
{
    private readonly Mock<IShipmentService> _mockShipmentService;
    private readonly Mock<ILogger<ShipmentController>> _mockLogger;
    private readonly ShipmentController _controller;

    public ShipmentControllerTests()
    {
        _mockShipmentService = new Mock<IShipmentService>();
        _mockLogger = new Mock<ILogger<ShipmentController>>();
        _controller = new ShipmentController(_mockShipmentService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllShipments()
    {
        // Arrange
        var mockShipments = new List<ShipmentDTO>
        {
            new ShipmentDTO("1", "Delivered", "New York", "Los Angeles", 100, DateTime.Now, DateTime.Now),
            new ShipmentDTO("2", "In Transit", "Chicago", "Miami", 200, DateTime.Now, null),
        };
        _mockShipmentService.Setup(service => service.GetAllAsync()).ReturnsAsync(mockShipments);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShipments = Assert.IsType<List<ShipmentDTO>>(okResult.Value);
        Assert.Equal(mockShipments.Count, returnedShipments.Count);
    }

    #region get-by-tracking-number-tests
    [Fact]
    public async Task GetByTrackingNumber_ReturnsShipment_WhenTrackingNumberIsValid()
    {
        // Arrange
        var trackingNumber = "1";
        var mockShipment = new ShipmentDTO(trackingNumber, "Delivered", "New York", "Los Angeles", 100, DateTime.Now, DateTime.Now);
        _mockShipmentService.Setup(service => service.GetByTrackingNumberAsync(trackingNumber)).ReturnsAsync(mockShipment);

        // Act
        var result = await _controller.GetByTrackingNumber(trackingNumber);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShipment = Assert.IsType<ShipmentDTO>(okResult.Value);
        Assert.Equal(mockShipment.TrackingNumber, returnedShipment.TrackingNumber);
    }

    [Fact]
    public async Task GetByTrackingNumber_ReturnsNotFound_WhenTrackingNumberIsInvalid()
    {
        // Arrange
        var trackingNumber = "invalid";
        _mockShipmentService.Setup(service => service.GetByTrackingNumberAsync(trackingNumber)).ReturnsAsync((ShipmentDTO)null);

        // Act
        var result = await _controller.GetByTrackingNumber(trackingNumber);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    #endregion

    #region add-shipment-tests
    [Fact]
    public async Task Add_ReturnsCreatedAtAction_WhenShipmentIsAddedSuccessfully()
    {
        // Arrange
        var newShipment = new ShipmentDTO("3", "In Transit", "Boston", "Seattle", 150, DateTime.Now, null);
        _mockShipmentService.Setup(service => service.AddAsync(newShipment));

        // Act
        var result = await _controller.Add(newShipment);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetByTrackingNumber", createdAtActionResult.ActionName); // Assuming the action name used in the Add method
        Assert.Equal(newShipment.TrackingNumber, ((ShipmentDTO)createdAtActionResult.Value).TrackingNumber);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenShipmentIsNotAdded()
    {
        // Arrange
        var newShipment = new ShipmentDTO("3", "In Transit", "Boston", "Seattle", 150, DateTime.Now, null);
        _mockShipmentService.Setup(service => service.AddAsync(newShipment)).ThrowsAsync(new InvalidOperationException("Shipment already exists"));

        // Act
        var result = await _controller.Add(newShipment);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region update-shipment-tests
    [Fact]
    public async Task Update_ValidUpdate_ReturnsOk()
    {
        // Arrange
        var trackingNumber = "123";
        // var shipment = new ShipmentDTO { TrackingNumber = trackingNumber };
        var updateRequest = new UpdateShipmentRequest { Status = "Delivered", Origin = "New York", Destination = "Los Angeles", Weight = 100, ShippingDate = DateTime.Now, DeliveryDate = DateTime.Now };
        _mockShipmentService.Setup(s => s.GetByTrackingNumberAsync(trackingNumber)).ReturnsAsync(new ShipmentDTO(trackingNumber, "In Transit", "Chicago", "Miami", 200, DateTime.Now, null));
        _mockShipmentService.Setup(s => s.UpdateAsync(It.IsAny<ShipmentDTO>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(updateRequest, trackingNumber);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockShipmentService.Verify(s => s.UpdateAsync(It.IsAny<ShipmentDTO>()), Times.Once);
    }
    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var invalidShipment = new UpdateShipmentRequest();
        var trackingNumber = "123";
        _controller.ModelState.AddModelError("TrackingNumber", "Tracking number is required");

        // Act
        var result = await _controller.Update(invalidShipment, trackingNumber);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region delete-shipment-tests

    [Fact]
    public async Task DeleteByTrackingNumber_ReturnsOk_WhenShipmentIsDeletedSuccessfully()
    {
        // Arrange
        var trackingNumber = "validTrackingNumber";
        _mockShipmentService.Setup(service => service.DeleteByTrackingNumberAsync(trackingNumber)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteByTrackingNumber(trackingNumber);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockShipmentService.Verify(service => service.DeleteByTrackingNumberAsync(trackingNumber), Times.Once);
    }

    [Fact]
    public async Task DeleteByTrackingNumber_ReturnsBadRequest_WhenInvalidOperationExceptionIsThrown()
    {
        // Arrange
        var trackingNumber = "invalidTrackingNumber";
        _mockShipmentService.Setup(service => service.DeleteByTrackingNumberAsync(trackingNumber)).ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        var result = await _controller.DeleteByTrackingNumber(trackingNumber);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid operation", badRequestResult.Value);
    }

    #endregion
}