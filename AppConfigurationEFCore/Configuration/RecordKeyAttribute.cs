namespace AppConfigurationEFCore.Configuration
{
    /// <summary>
    /// Attribute, that has to be present at each property of <c>TRecords</c> of <see cref="IAppConfiguration{TRecords}"/>.
    /// </summary>
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
