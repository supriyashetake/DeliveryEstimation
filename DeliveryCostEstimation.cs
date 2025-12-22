using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
		Console.WriteLine("<baseCost> <packageCount>");
        string[] header = Console.ReadLine().Split(' ');
        double baseCost = double.Parse(header[0]);
        int packageCount = int.Parse(header[1]);

        for (int i = 0; i < packageCount; i++)
        {
			Console.WriteLine("<id> <weight> <distance> <offerCode>");
            string[] pkgData = Console.ReadLine().Split(' ');
            string id = pkgData[0];
            double weight = double.Parse(pkgData[1]);
            double distance = double.Parse(pkgData[2]);
            string offerCode = pkgData[3];

            double deliveryCost = baseCost + (weight * 10) + (distance * 5);
            double discountPercentage = GetDiscount(offerCode, weight, distance);
            double discountAmount = Math.Floor(deliveryCost * discountPercentage);
            double totalCost = deliveryCost - discountAmount;

            Console.WriteLine($"{id} {discountAmount} {totalCost}");
        }
    }

    private static double GetDiscount(string code, double weight, double distance)
    {
        return code switch
        {
            "OFR001" when distance < 200 && weight >= 70 && weight <= 200 => 0.10,
            "OFR002" when distance >= 50 && distance <= 150 && weight >= 100 && weight <= 250 => 0.07,
            "OFR003" when distance >= 50 && distance <= 250 && weight >= 10 && weight <= 150 => 0.05,
            _ => 0.0
        };
    }
}
