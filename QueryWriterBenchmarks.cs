using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace WriteQueryTos3;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class QueryWriterBenchmarks
{
    [Benchmark]
    public async Task WriteToFileOnDisk()
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "cars.csv");

        using (var context = new AppDbContext())
        {
            var query = context.Cars
                .AsNoTracking()
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model);

            var cars = await query.ToListAsync();
            
            await using var textWriter = new StreamWriter(filePath, true);
            var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
            await csvWriter.WriteRecordsAsync(cars);
        }
    }
    
    [Benchmark]
    public async Task WriteToFileOnDiskInBatches()
    {
        const int pageSize = 10;
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "cars.csv");

        using (var context = new AppDbContext())
        {
            var query = context.Cars
                .AsNoTracking()
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model);

            var totalCount = await query.CountAsync();

            var numPages = (totalCount + pageSize) / pageSize;

            for (var i = 0; i < numPages; i++)
            {
                var cars = query.Skip(i).Take(pageSize);

                await using var textWriter = new StreamWriter(filePath, true);
                var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
                await csvWriter.WriteRecordsAsync(cars);
            }
        }
    }
}