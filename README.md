# AppConfigurationEFCore
Configurable, type-aware service for storing application configuration in database table.

If you would like to see commits before repository upload, go to SchoolAssistant repo, branch 'AppConfigRepository_as_separate_project'

### Dependencies
Entity Framework Core 6.0.4,

Microsoft.Extensions.DependencyInjection.Abstractions


## Usage
###### Table in database and DbSet<>
It is important to add `DbSet<AppConfig>` to used `DbContext` (and create database table with migration, `Add-Migration AppConfiguration` in console):
```
public class YourDbContext : DbContext {
  public SADbContext(DbContextOptions<SADbContext> options) : base(options) { }
  
  ...
  
  protected DbSet<AppConfigurationEFCore.Entities.AppConfig> _Config { get; set; } = null!;
  ...
}
```

###### Library setup
In `Program.cs` add `using AppConfigurationEFCore.Setup;` at the top and `budilder.Services.AddAppConfiguration<YourDbContext, YourRecordsConfiguration>(options => { ... });` somewhere below:
```
using AppConfigurationEFCore.Setup;
...
using Microsoft.EntityFrameworkCore;
...

builder.Services.AddAppConfiguration<YourDbContext, YourRecordsConfiguration>(options =>
{
    options.AddVT<DateTime>(x =>
    {
        if (DateTime.TryParse(x, out var date))
            return date;
        return null;
    });
});
```

###### Records configuration
Type `YourRecordsConfiguration` is where you define records used in your application. There are rules, how this class should be structured (it is also explained in summary of `AddAppConfiguration` method):
`TRecords` type must have properties of type `RecordHandler<T>` (for records that represent reference types)
or `VTRecordHandler<T>` (for records that represent value types, like `int`, `decimal`).

Each property must have attribute `RecordKeyAttribute` with the key of that record.

Example:
```
public class AppConfigRecords 
{
    [RecordKey("name")]
    public RecordHandler<string> ApplicationName { get; private set; } = null!;
    [RecordKey("maxItemsPerPage")]
    public VTRecordHandler<int> MaxItemsPerPage { get; private set; } = null!;
    [RecordKey("defaultValueOfSth")]
    public VTRecordHandler<int> SomeDefaultValue { get; private set; } = null!;
    [RecordKey("initialDate")]
    public VTRecordHandler<DateTime> InitialDate { get; private set; } = null!;
    ...
}
```

###### Record types
By deafult **only `int`, `decimal` and `string` record handlers are available**. To add new use `customRecordTypesAction` parameter in `AddAppConfiguration`. You can add support for your own reference types (with `Add<T>(...)` method) or readonly structs (with `AddVT<T>(...)`). Those methods require conversion function from `string?` to `[yourType]?`, the other way is optional (`[...].ToString()` will be used by default).

You can also pass, instead of function, object, which class implements interface `IRecordHandlerRule<T>` or `IVTRecordHandlerRule<T>`:
```
class CharHandler : IVTRecordHandlerRule<char>
{
    public string? FromType(char? en) => en is not null ? $"{en}" : null;

    public char? ToType(string? db) => db?.FirstOrDefault();
}

...
budilder.Services.AddAppConfiguration<YourDbContext, YourRecordsConfiguration>(options => {
    options.AddVT(new CharHandler());
});
```
or even built-in `JsonRecordHandlerRule<T>`:
```
private class SimpleClass
{
    public string Name { get; set; }
    public int Number { get; set; }
}

...
budilder.Services.AddAppConfiguration<YourDbContext, YourRecordsConfiguration>(options => {
    options.Add(new JsonRecordHandlerRule<SimpleClass>());
});
```
