namespace AppConfigurationEFCore.Configuration
{
    /// <summary> Attribute indicating record. </summary>
    /// <remarks>
    /// Attribute has to be present on properties of <c>TRecords</c> of <see cref="IAppConfiguration{TRecords}"/>,
    /// that are of type <see cref="RecordHandler{T}"/> or <see cref="VTRecordHandler{T}"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RecordKeyAttribute : Attribute
    {
        public string Key { get; }
        public RecordKeyAttribute(string key)
        {
            Key = key;
        }
    }
}
