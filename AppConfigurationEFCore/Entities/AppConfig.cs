using System.ComponentModel.DataAnnotations;

namespace AppConfigurationEFCore.Entities
{
    /// <summary>
    /// Database entity, all application configuration records are stored in form of this class.
    /// </summary>
    public class AppConfig
    {
        [Key]
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}
