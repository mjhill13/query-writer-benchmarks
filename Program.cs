﻿using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using WriteQueryTos3;

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