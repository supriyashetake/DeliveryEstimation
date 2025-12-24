using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

public class DeliveryTimeTests
{
    // ---------------- PRICE CALCULATION TESTS ----------------

    [Fact]
    public void CalculatePrice_OFR001_ValidDiscount()
    {
        var pkg = new DeliveryApp.Package
        {
            Id = "PKG1",
            Weight = 100,
            Distance = 100,
            Offer = "OFR001"
        };

        InvokeCalculatePrice(pkg, 100);

        Assert.Equal(160, pkg.Discount);
        Assert.Equal(1440, pkg.TotalCost);
    }

    [Fact]
    public void CalculatePrice_InvalidOffer_NoDiscount()
    {
        var pkg = new DeliveryApp.Package
        {
            Id = "PKG2",
            Weight = 50,
            Distance = 50,
            Offer = "INVALID"
        };

        InvokeCalculatePrice(pkg, 100);

        Assert.Equal(0, pkg.Discount);
        Assert.Equal(850, pkg.TotalCost);
    }

    [Fact]
    public void CalculatePrice_OFR003_BoundaryCondition()
    {
        var pkg = new DeliveryApp.Package
        {
            Id = "PKG3",
            Weight = 10,
            Distance = 50,
            Offer = "OFR003"
        };

        InvokeCalculatePrice(pkg, 100);

        Assert.True(pkg.Discount > 0);
    }

    // ---------------- BATCH SELECTION TESTS ----------------

    [Fact]
    public void GetBestBatch_MaxPackagesWithinMaxLoad()
    {
        var packages = new List<DeliveryApp.Package>
        {
            new DeliveryApp.Package { Id = "P1", Weight = 50 },
            new DeliveryApp.Package { Id = "P2", Weight = 75 },
            new DeliveryApp.Package { Id = "P3", Weight = 150 }
        };

        var batch = InvokeGetBestBatch(packages, 200);

        Assert.Equal(2, batch.Count);
        Assert.True(batch.Sum(p => p.Weight) <= 200);
    }

    [Fact]
    public void GetBestBatch_PrefersHigherWeightWhenSameCount()
    {
        var packages = new List<DeliveryApp.Package>
        {
            new DeliveryApp.Package { Id = "P1", Weight = 30 },
            new DeliveryApp.Package { Id = "P2", Weight = 40 },
            new DeliveryApp.Package { Id = "P3", Weight = 70 }
        };

        var batch = InvokeGetBestBatch(packages, 100);

        Assert.Equal(2, batch.Count);
        Assert.Equal(70, batch.Sum(p => p.Weight));
    }

    // ---------------- DELIVERY TIME TEST (INTEGRATION) ----------------

    [Fact]
    public void Main_FullFlow_ProducesExpectedOutput()
    {
        string input =
@"100 1
PKG1 100 100 OFR001
1 70 200";

        var reader = new StringReader(input);
        var writer = new StringWriter();

        Console.SetIn(reader);
        Console.SetOut(writer);

        DeliveryApp.Main();

        string output = writer.ToString();

        Assert.Contains("PKG1", output);
        Assert.Contains("160", output);
        Assert.Contains("1440", output);
    }

    // ---------------- REFLECTION HELPERS ----------------

    private static void InvokeCalculatePrice(DeliveryApp.Package pkg, double baseCost)
    {
        var method = typeof(DeliveryApp)
            .GetMethod("CalculatePrice", BindingFlags.NonPublic | BindingFlags.Static);

        method.Invoke(null, new object[] { pkg, baseCost });
    }

    private static List<DeliveryApp.Package> InvokeGetBestBatch(
        List<DeliveryApp.Package> pkgs, double maxLoad)
    {
        var method = typeof(DeliveryApp)
            .GetMethod("GetBestBatch", BindingFlags.NonPublic | BindingFlags.Static);

        return (List<DeliveryApp.Package>)method.Invoke(null, new object[] { pkgs, maxLoad });
    }
}
