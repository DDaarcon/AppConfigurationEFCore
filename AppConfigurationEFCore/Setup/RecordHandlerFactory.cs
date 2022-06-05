using AppConfigurationEFCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AppConfigurationEFCore.Setup
{
    internal interface IRecordHandlerFactory
    {
        object? Get(Type type, string key, Func<DbContext> getContext);
        object? GetVT(Type type, string key, Func<DbContext> getContext);
    }
    internal class RecordHandlerFactory : IRecordHandlerFactory
    {
        private readonly IList<string> _usedKeys = new List<string>();
        private readonly object[] _handlersInfo;
        private readonly object[] _vtHandlersInfo;

        private Type _type = null!;
        private string _key = null!;
        private Func<DbContext> _getContext = null!;

        public RecordHandlerFactory(
            object[] info,
            object[] vtInfo)
        {
            _handlersInfo = info;
            _vtHandlersInfo = vtInfo;
        }

        public object? Get(Type type, string key, Func<DbContext> getContext)
        {
            if (!AssignHandlerParamsAndValidateNulls(type, key, getContext))
                return null;

            if (TryGetDefaultHandler(out var h1))
                return h1;

            if (TryGetUserDefinedHandler(out var h2))
                return h2;

            return null;
        }

        public object? GetVT(Type type, string key, Func<DbContext> getContext)
        {
            if (!AssignHandlerParamsAndValidateNulls(type, key, getContext))
                return null;

            if (TryGetDefaultVTHandler(out var h1))
                return h1;

            if (TryGetUserDefinedVTHandler(out var h2))
                return h2;

            return null;
        }

        private bool AssignHandlerParamsAndValidateNulls(Type type, string key, Func<DbContext> getContext)
        {
            _type = type; _key = key; _getContext = getContext;
            if (_type is null) return false;
            if (_key is null) return false;
            if (_getContext is null) return false;

            if (_usedKeys.Contains(_key))
                throw new Exception($"Duplicate usage of one key. Key '{_key}' is used twice in application configuration records.");
            _usedKeys.Add(_key);

            return true;
        }

        private bool TryGetDefaultHandler(out object? handlerObj)
        {
            handlerObj = null;

            if (typeof(string).IsEquivalentTo(_type))
            {
                handlerObj = new RecordHandler<string>(
                    _key, _getContext,
                    to => to, from => from);
                return true;
            }

            return false;
        }

        private bool TryGetUserDefinedHandler(out object? handlerObj)
        {
            var methodParams = new object?[] { null };
            var result = this.GetType()
                .GetMethod("TryGetUserDefinedHandler")!
                .MakeGenericMethod(_type)
                .Invoke(this, methodParams) as bool? ?? false;
            handlerObj = methodParams[0];
            return result;
        }
        public bool TryGetUserDefinedHandler<T>(out RecordHandler<T>? handlerObj)
        {
            handlerObj = null;
            var info = _handlersInfo.FirstOrDefault(x => x is HandlerInfo<T> h && h.ForType == _type) as HandlerInfo<T>;

            if (info is null) return false;

            handlerObj = new RecordHandler<T>(
                _key, _getContext,
                info.ToTypeConverter, info.FromTypeConverter);

            return true;
        }




        private bool TryGetDefaultVTHandler(out object? handlerObj)
        {
            handlerObj = null;

            if (typeof(int).IsEquivalentTo(_type))
            {
                handlerObj = new VTRecordHandler<int>(
                    _key, _getContext,
                    to => to is null ? null : int.Parse(to), from => from?.ToString());
                return true;
            }
            if (typeof(decimal).IsEquivalentTo(_type))
            {
                handlerObj = new VTRecordHandler<decimal>(
                    _key, _getContext,
                    to => to is null ? null : decimal.Parse(to), from => from?.ToString());
                return true;
            }

            return false;
        }

        private bool TryGetUserDefinedVTHandler(out object? handlerObj)
        {
            var methodParams = new object?[] { null };
            var result = this.GetType()
                .GetMethod("TryGetUserDefinedVTHandler")!
                .MakeGenericMethod(_type)
                .Invoke(this, methodParams) as bool? ?? false;
            handlerObj = methodParams[0];
            return result;
        }
        public bool TryGetUserDefinedVTHandler<T>(out VTRecordHandler<T>? handlerObj)
            where T : struct
        {
            handlerObj = null;
            var info = _vtHandlersInfo.FirstOrDefault(x => x is VTHandlerInfo<T> h && h.ForType == _type) as VTHandlerInfo<T>;

            if (info is null) return false;

            handlerObj = new VTRecordHandler<T>(
                _key, _getContext,
                info.ToTypeConverter, info.FromTypeConverter);

            return true;
        }

    }
}
