using WriteQueryTos3;

using var context = new AppDbContext();
{
    context.Database.EnsureCreated();    

    BenchmarkRunner.Run<QueryWriterBenchmarks>();
}