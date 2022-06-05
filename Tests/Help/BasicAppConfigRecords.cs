using AppConfigurationEFCore.Configuration;

namespace Tests
{
    internal class BasicAppConfigRecords
    {
        [RecordKey("name")]
        public RecordHandler<string> Name { get; set; } = null!;

        [RecordKey("number")]
        public VTRecordHandler<int> SomeNumber { get; set; } = null!;

        [RecordKey("floatingPointNumber")]
        public VTRecordHandler<decimal> SomeFloatingPointNumber { get; set; } = null!;
    }
}
