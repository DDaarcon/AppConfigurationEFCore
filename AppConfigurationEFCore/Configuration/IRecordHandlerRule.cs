namespace AppConfigurationEFCore.Configuration
{
    public interface IRecordHandlerRule<T>
    {
        T? ToType(string? db);
        string? FromType(T? en);
    }

    public interface IVTRecordHandlerRule<T>
        where T : struct
    {
        T? ToType(string? db);
        string? FromType(T? en);
    }
}
