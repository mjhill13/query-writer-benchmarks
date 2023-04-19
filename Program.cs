using System.Globalization;
using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CsvHelper;
using CsvHelper.Configuration;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using WriteQueryTos3;

using var context = new AppDbContext();
{
    context.Database.EnsureCreated();    

    BenchmarkRunner.Run<QueryWriterBenchmarks>();
}

// const string bucketName = "my-cars";
// const string fileName = "cars.csv";
//
// var s3rver = new ContainerBuilder()
//     .WithImage("jbergknoff/s3rver")
//     .WithPortBinding(5000, true)
//     .Build();
//
// try
// {
//     await s3rver.StartAsync();
//     
//     await using var context = new AppDbContext();
//     var query = context.Cars
//         .AsNoTracking()
//         .OrderBy(x => x.Make)
//         .ThenBy(x => x.Model);
//
//     var cars = await query.ToListAsync();
//
//     await using (var memoryStream = new MemoryStream())
//     {
//         var streamWriter = new StreamWriter(memoryStream);
//         var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
//         await csvWriter.WriteRecordsAsync(cars);
//
//         var config = new AmazonS3Config
//         {
//             ForcePathStyle = true,
//             UseHttp = true,
//             ServiceURL = $"http://localhost:{s3rver.GetMappedPublicPort(5000)}"
//         };
//         AWSCredentials creds = new BasicAWSCredentials("S3RVER", "S3RVER");
//         AmazonS3Client s3Client = new AmazonS3Client(creds, config);
//
//         await s3Client.PutBucketAsync(bucketName);
//         
//         using TransferUtility tranUtility = new TransferUtility(s3Client);
//         tranUtility.Upload(memoryStream, bucketName, fileName);
//
//         var file = await s3Client.GetObjectAsync(new GetObjectRequest
//         {
//             BucketName = bucketName,
//             Key = fileName
//         });
//
//         var message = new StringBuilder();
//         message.AppendLine("Successfully uploaded:");
//         message.AppendLine($"Key: {file.Key}");
//         message.AppendLine($"Last Modified: {file.LastModified}");
//         Console.WriteLine(message);
//     }
// }
// finally
// {
//     await s3rver.StopAsync();
// }