using System;
using System.Collections.Generic;
using System.Linq;

public class DeliveryApp
{
    public class Package
    {
        public string Id;
        public double Weight;
        public double Distance;
        public string Offer;
        public double Discount;
        public double TotalCost;
        public double DeliveryTime;
    }

    public static void Main()
    {
        // ---------- INPUT ----------
		Console.WriteLine("<base_delivery_cost> <number_of_packages>");
        var input1 = Console.ReadLine().Split();
        double baseCost = double.Parse(input1[0]);
        int nPkgs = int.Parse(input1[1]);

        List<Package> packages = new List<Package>();
        for (int i = 0; i < nPkgs; i++)
        {
			Console.WriteLine("<package_id> <weight_in_kg> <distance_in_km> <offer_code>");
            var p = Console.ReadLine().Split();
            packages.Add(new Package
            {
                Id = p[0],
                Weight = double.Parse(p[1]),
                Distance = double.Parse(p[2]),
                Offer = p[3]
            });
        }

        var input2 = Console.ReadLine().Split();
        int nVehicles = int.Parse(input2[0]);
        double maxSpeed = double.Parse(input2[1]);
        double maxLoad = double.Parse(input2[2]);

        // ---------- COST CALCULATION ----------
        foreach (var p in packages)
            CalculatePrice(p, baseCost);

        // ---------- DELIVERY SCHEDULING ----------
        double[] vehicleAvailableAt = new double[nVehicles];
        List<Package> remaining = new List<Package>(packages);

        while (remaining.Count > 0)
        {
            var batch = GetBestBatch(remaining, maxLoad);

            int vehicleIndex = Array.IndexOf(vehicleAvailableAt, vehicleAvailableAt.Min());
            double startTime = vehicleAvailableAt[vehicleIndex];

            double maxTripTime = 0;

            foreach (var p in batch)
            {
                double travelTime = p.Distance / maxSpeed;
                p.DeliveryTime = Math.Round(startTime + travelTime, 2);
                maxTripTime = Math.Max(maxTripTime, travelTime);
                remaining.Remove(p);
            }

            vehicleAvailableAt[vehicleIndex] += 2 * maxTripTime;
        }

        // ---------- OUTPUT ----------
        foreach (var p in packages)
            Console.WriteLine($"{p.Id} {p.Discount} {p.TotalCost} {p.DeliveryTime}");
    }

    // ---------- PRICE LOGIC ----------
    private static void CalculatePrice(Package p, double baseCost)
    {
        double deliveryCost = baseCost + (p.Weight * 10) + (p.Distance * 5);
        double discountRate = GetDiscount(p.Offer, p.Weight, p.Distance);

        p.Discount = Math.Floor(deliveryCost * discountRate);
        p.TotalCost = deliveryCost - p.Discount;
    }

    private static double GetDiscount(string code, double weight, double distance)
    {
        if (code == "OFR001" && distance < 200 && weight >= 70 && weight <= 200)
            return 0.10;
        if (code == "OFR002" && distance >= 50 && distance <= 150 && weight >= 100 && weight <= 250)
            return 0.07;
        if (code == "OFR003" && distance >= 50 && distance <= 250 && weight >= 10 && weight <= 150)
            return 0.05;
        return 0.0;
    }

    // ---------- CORRECT BATCH SELECTION ----------
    // Max packages → Max weight (within maxLoad)
    private static List<Package> GetBestBatch(List<Package> pkgs, double maxLoad)
    {
        List<Package> best = new List<Package>();
        int n = pkgs.Count;

        for (int mask = 1; mask < (1 << n); mask++)
        {
            double totalWeight = 0;
            List<Package> current = new List<Package>();

            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    totalWeight += pkgs[i].Weight;
                    current.Add(pkgs[i]);
                }
            }

            if (totalWeight <= maxLoad)
            {
                if (current.Count > best.Count ||
                   (current.Count == best.Count &&
                    current.Sum(p => p.Weight) > best.Sum(p => p.Weight)))
                {
                    best = current;
                }
            }
        }
        return best;
    }
}
