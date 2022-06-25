using AppConfigurationEFCore.Configuration;
using System.Collections;

namespace AppConfigurationEFCore.Setup
{
    public sealed class CustomRecordTypeOptions
    {

        #region Adding custom record handlers

        /// <summary>
        /// Register reference type handler.
        /// </summary>
        /// <typeparam name="T">Reference type</typeparam>
        /// <param name="toTypeConverter">Function converting <c>string</c> value to <typeparamref name="T"/>.</param>
        /// <param name="fromTypeConverter">Function converting <typeparamref name="T"/> to <c>string</c>. By default <c>ToString()</c> method will be used.</param>
        public void Add<T>(Func<string?, T?> toTypeConverter, Func<T?, string?>? fromTypeConverter = null)
        {
            var type = typeof(T);
            _info.Add(new HandlerInfo<T>(type, toTypeConverter, fromTypeConverter ?? (x => x?.ToString())));
        }

        /// <summary>
        /// Register reference type handler via object with converting methods.
        /// </summary>
        /// <typeparam name="T">Reference type</typeparam>
        public void Add<T>(IRecordHandlerRule<T> rules) => Add(rules.ToType, rules.FromType);


        /// <summary>
        /// Register value type handler.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="toTypeConverter">Function converting <c>string</c> value to <typeparamref name="T"/>.</param>
        /// <param name="fromTypeConverter">Function converting <typeparamref name="T"/> to <c>string</c>. By default <c>ToString()</c> method will be used.</param>
        public void AddVT<T>(Func<string?, T?> toTypeConverter, Func<T?, string?>? fromTypeConverter = null)
            where T : struct
        {
            var type = typeof(T);
            _vtInfo.Add(new VTHandlerInfo<T>(type, toTypeConverter, fromTypeConverter ?? (x => x?.ToString())));
        }

        /// <summary>
        /// Register value type handler via object with converting methods.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        public void AddVT<T>(IVTRecordHandlerRule<T> rules)
            where T : struct
            => AddVT(rules.ToType, rules.FromType);


        public object[] ReferenceTypeHandlers => _info.ToArray()!;
        public object[] VTTypeHandlers => _vtInfo.ToArray()!;

        private readonly ArrayList _info = new();

        private readonly ArrayList _vtInfo = new();
    }



    internal record HandlerInfo<T>(
        Type ForType,
        Func<string?, T?> ToTypeConverter,
        Func<T?, string?> FromTypeConverter);

    internal record VTHandlerInfo<T>(
        Type ForType,
        Func<string?, T?> ToTypeConverter,
        Func<T?, string?> FromTypeConverter)
        where T : struct;
}
