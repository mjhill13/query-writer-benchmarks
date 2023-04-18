// See https://aka.ms/new-console-template for more information

using WriteQueryTos3;

using (var context = new AppDbContext())
{
    var query = context.Cars
        .OrderBy(x => x.Make)
        .ThenBy(x => x.Model);
    
    
}