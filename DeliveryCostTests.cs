using Xunit;

public class DeliveryCostTests
{
    // ---------------- OFR001 ----------------
    [Fact]
    public void OFR001_ValidCondition_Apply10PercentDiscount()
    {
        double baseCost = 100;
        double weight = 100;
        double distance = 100;
        string offerCode = "OFR001";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(160, result.discount);
        Assert.Equal(1440, result.total);
    }

    [Fact]
    public void OFR001_InvalidWeight_NoDiscount()
    {
        double baseCost = 100;
        double weight = 60; // below limit
        double distance = 150;
        string offerCode = "OFR001";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(0, result.discount);
        Assert.Equal(1150, result.total);
    }

    // ---------------- OFR002 ----------------
    [Fact]
    public void OFR002_ValidCondition_Apply7PercentDiscount()
    {
        double baseCost = 100;
        double weight = 150;
        double distance = 100;
        string offerCode = "OFR002";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(112, result.discount);
        Assert.Equal(1488, result.total);
    }

    [Fact]
    public void OFR002_InvalidDistance_NoDiscount()
    {
        double baseCost = 100;
        double weight = 150;
        double distance = 200; // out of range
        string offerCode = "OFR002";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(0, result.discount);
        Assert.Equal(1600, result.total);
    }

    // ---------------- OFR003 ----------------
    [Fact]
    public void OFR003_ValidCondition_Apply5PercentDiscount()
    {
        double baseCost = 100;
        double weight = 50;
        double distance = 200;
        string offerCode = "OFR003";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(80, result.discount);
        Assert.Equal(1520, result.total);
    }

    [Fact]
    public void OFR003_InvalidWeight_NoDiscount()
    {
        double baseCost = 100;
        double weight = 200; // above limit
        double distance = 100;
        string offerCode = "OFR003";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(0, result.discount);
        Assert.Equal(1600, result.total);
    }

    // ---------------- Invalid Offer ----------------
    [Fact]
    public void InvalidOfferCode_NoDiscount()
    {
        double baseCost = 100;
        double weight = 10;
        double distance = 10;
        string offerCode = "INVALID";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.Equal(0, result.discount);
        Assert.Equal(250, result.total);
    }

    // ---------------- Boundary Condition ----------------
    [Fact]
    public void BoundaryCondition_ExactLimit_ShouldApplyDiscount()
    {
        double baseCost = 100;
        double weight = 70;   // exact boundary
        double distance = 199;
        string offerCode = "OFR001";

        var result = Program.CalculateCost(baseCost, weight, distance, offerCode);

        Assert.True(result.discount > 0);
    }
}
