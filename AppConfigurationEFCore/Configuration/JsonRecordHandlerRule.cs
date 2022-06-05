using System.Text.Json;

namespace AppConfigurationEFCore.Configuration
{
    public class JsonRecordHandlerRule<T> : IRecordHandlerRule<T>
    {
        public string? FromType(T? en) => en is not null ? JsonSerializer.Serialize(en) : null;

        public T? ToType(string? db) => db is not null ? JsonSerializer.Deserialize<T>(db) : default;
    }
}
