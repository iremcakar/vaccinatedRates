using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Read input file
        string[] lines = File.ReadAllLines("input.txt");

        // Parse data into a list of tuples
        List<(string, DateTime, int?, string)> data = new List<(string, DateTime, int?, string)>();
        foreach (string line in lines.Skip(1))
        {
            string[] parts = line.Split('\t');
            string country = parts[0];
            DateTime date = DateTime.Parse(parts[1]);
            int? dailyVaccinations = string.IsNullOrEmpty(parts[2]) ? null : (int?)int.Parse(parts[2]);
            string vaccine = parts[3];
            data.Add((country, date, dailyVaccinations, vaccine));
        }

        // Answer question 1: Which country has administered the most total vaccines?
        var totalVaccinesByCountry = data.GroupBy(d => d.Item1)
            .Select(g => new { Country = g.Key, TotalVaccinations = g.Sum(d => d.Item3 ?? 0) })
            .OrderByDescending(x => x.TotalVaccinations);
        Console.WriteLine($"1. {totalVaccinesByCountry.First().Country}");

        // Answer question 2: What is the average daily vaccination rate of each vaccine type?
        var avgDailyVaccinationsByVaccine = data.GroupBy(d => d.Item4)
            .Select(g => new { Vaccine = g.Key, AvgDailyVaccinations = g.Where(d => d.Item3.HasValue).Average(d => d.Item3) })
            .OrderByDescending(x => x.AvgDailyVaccinations);
        foreach (var item in avgDailyVaccinationsByVaccine)
        {
            Console.WriteLine($"2. {item.Vaccine}: {item.AvgDailyVaccinations}");
        }

        // Answer question 3: What is the date on which the country with the highest daily vaccinations recorded its highest daily vaccinations?
        var maxDailyVaccinationsByCountry = data.GroupBy(d => d.Item1)
            .Select(g => new { Country = g.Key, MaxDailyVaccinations = g.Max(d => d.Item3 ?? 0) });
        var dateOfHighestDailyVaccinations = data.Where(d => d.Item3.HasValue)
            .Join(maxDailyVaccinationsByCountry, d => d.Item1, c => c.Country, (d, c) => new { d.Item1, d.Item2, d.Item3, d.Item4, c.MaxDailyVaccinations })
            .Where(x => x.Item3 == x.MaxDailyVaccinations)
            .OrderByDescending(x => x.MaxDailyVaccinations)
            .Select(x => x.Item2)
            .First();
        Console.WriteLine($"3. {dateOfHighestDailyVaccinations:MM/dd/yyyy}");

        Console.ReadLine();
    }
}
