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
public class QueryWriterBenchmarks : IDisposable
{
    private readonly AppDbContext _context = new();
    
    [Benchmark]
    public async Task WriteToFileOnDisk()
    {
        // string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "cars.csv");
        string filePath = "~/dev/cars.csv";

        var query = _context.Cars
            .AsNoTracking()
            .OrderBy(x => x.Make)
            .ThenBy(x => x.Model);

        var cars = await query.ToListAsync();
        
        await using var textWriter = new StreamWriter(filePath, true);
        var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
        await csvWriter.WriteRecordsAsync(cars);
    }
    
    // [Benchmark]
    public async Task WriteToFileOnDiskInBatches()
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "cars-batch.csv");
        
        var query = _context.Cars
            .AsNoTracking()
            .OrderBy(x => x.Make)
            .ThenBy(x => x.Model);

        var totalCount = await query.CountAsync();
        
        var numPages = (totalCount + Consts.BatchSize) / Consts.BatchSize;

        for (var i = 0; i < numPages; i++)
        {
            var cars = query.Skip(i).Take(Consts.BatchSize);
            
            await using var textWriter = new StreamWriter(filePath, true);
            var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
            await csvWriter.WriteRecordsAsync(cars);
        }
    }
    
    [Benchmark]
    public async Task WriteToFileOnDiskWithQueryable()
    {
        // string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "cars-queryable.csv");
        string filePath = "~/dev/cars-queryable.csv";
        
        var query = _context.Cars
            .AsNoTracking()
            .OrderBy(x => x.Make)
            .ThenBy(x => x.Model);
        
        await using var textWriter = new StreamWriter(filePath, true);
        var csvWriter = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
        foreach (var car in query) csvWriter.WriteRecord(car);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}