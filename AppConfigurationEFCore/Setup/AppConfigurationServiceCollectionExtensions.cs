using AppConfigurationEFCore.Configuration;
using AppConfigurationEFCore.Help;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppConfigurationEFCore.Setup
{
    public static class AppConfigurationServiceCollectionExtensions
    {
        /// <summary>
        /// Register <see cref="IAppConfiguration{TRecords}"/> factory.
        /// </summary>
        /// <remarks>
        /// 
        /// <typeparamref name="TRecords"/> type must have properties of type <see cref="RecordHandler{T}"/> (for records that come from reference types)
        /// <br />
        /// or <see cref="VTRecordHandler{T}"/> (for records that come from value types, like <c>int</c>, <c>decimal</c>).
        /// <br />
        /// Each property must have attribute <see cref="RecordKeyAttribute"/> with key of configuration record.
        /// <br />
        /// Example:
        /// <code>
        /// public class AppConfigRecords 
        /// {
        ///     [RecordKey("name")]
        ///     public RecordHandler&lt;string&gt; ApplicationName { get; private set; } = null!;
        ///     [RecordKey("maxItemsPerPage")]
        ///     public VTRecordHandler&lt;int&gt; MaxItemsPerPage { get; private set; } = null!;
        /// }
        /// </code>
        /// <br />
        /// <b>IMPORTANT</b>
        /// <br />
        /// By default only handlers for types <c>string</c>, <c>int</c> and <c>decimal</c> are registered. To register your own use <paramref name="customRecordTypesAction"/>.
        /// </remarks>
        /// <typeparam name="TDbContext"> EF Core DbContext used in your application </typeparam>
        /// <typeparam name="TRecords">
        /// Type with defined records that you'd like to have in your AppConfiguration table. See remarks for an example.
        /// </typeparam>
        /// <param name="customRecordTypesAction">
        /// Use this action to configure your own type handler.
        /// Method <see cref="CustomRecordTypeOptions.Add{T}(Func{string?, T?}, Func{T?, string?}?)"/> registers reference type converter,
        /// <see cref="CustomRecordTypeOptions.AddVT{T}(Func{string?, T?}, Func{T?, string?}?)"/> registers value type converter.
        /// </param>
        public static IServiceCollection AddAppConfiguration<TDbContext, TRecords>(this IServiceCollection services, Action<CustomRecordTypeOptions>? customRecordTypesAction = null)
            where TDbContext : DbContext
            where TRecords : class, new()
        {
            return AddAppConfiguration(services, typeof(TDbContext), typeof(TRecords), customRecordTypesAction);
        }








        /// <summary>
        /// Register <see cref="IAppConfiguration{TRecords}"/> factory.
        /// </summary>
        /// <remarks>
        /// 
        /// <c>TRecords</c> type must have properties of type <see cref="RecordHandler{T}"/> (for records that come from reference types)
        /// <br />
        /// or <see cref="VTRecordHandler{T}"/> (for records that come from value types, like <c>int</c>, <c>decimal</c>).
        /// <br />
        /// Each property must have attribute <see cref="RecordKeyAttribute"/> with key of configuration record.
        /// <br />
        /// Example:
        /// <code>
        /// public class AppConfigRecords 
        /// {
        ///     [RecordKey("name")]
        ///     public RecordHandler&lt;string&gt; ApplicationName { get; private set; } = null!;
        ///     [RecordKey("maxItemsPerPage")]
        ///     public VTRecordHandler&lt;int&gt; MaxItemsPerPage { get; private set; } = null!;
        /// }
        /// </code>
        /// <br />
        /// <b>IMPORTANT</b>
        /// <br />
        /// By default only handlers for types <c>string</c>, <c>int</c> and <c>decimal</c> are registered. To register your own use <paramref name="customRecordTypesAction"/>.
        /// </remarks>
        /// <param name="dbContextType"> EF Core DbContext used in your application </param>
        /// <param name="configurationRecordsType">
        /// Type with defined records that you'd like to have in your AppConfiguration table. See remarks for an example.
        /// </param>
        /// <param name="customRecordTypesAction">
        /// Use this action to configure your own type handler.
        /// Method <see cref="CustomRecordTypeOptions.Add{T}(Func{string?, T?}, Func{T?, string?}?)"/> registers reference type converter,
        /// <see cref="CustomRecordTypeOptions.AddVT{T}(Func{string?, T?}, Func{T?, string?}?)"/> registers value type converter.
        /// </param>
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, Type dbContextType, Type configurationRecordsType, Action<CustomRecordTypeOptions>? customRecordTypesAction = null)
        {
            var svc = new RegisterFactoriesService();
            svc.Register(services, dbContextType, configurationRecordsType, customRecordTypesAction);

            return services;
        }
    }
}
