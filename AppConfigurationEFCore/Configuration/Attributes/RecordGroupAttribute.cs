using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfigurationEFCore.Configuration
{
    /// <summary>
    /// Attribute indiacting group of records.
    /// </summary>
    /// <remarks>
    /// Attribute has to be present on class representing group of records, or on property of Records Configuration class
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class RecordGroupAttribute : Attribute
    {
        /// <summary> Prefix for each record in group </summary>
        public string? GroupKey { get; set; }

        /// <summary> Separator between <see cref="GroupKey"/> and record key </summary>
        /// <remarks> Default is <c>.</c> </remarks>
        public string Separator { get; set; } = ".";
    }
}
