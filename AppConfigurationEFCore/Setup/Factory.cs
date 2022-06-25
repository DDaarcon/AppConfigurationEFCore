using AppConfigurationEFCore.Help;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AppConfigurationEFCore.Setup
{
    internal class Factory<TDbContext, TRecords>
        where TDbContext : DbContext
        where TRecords : class, new()
    {
        private readonly IRecordHandlerFactory _handlerFactory;
        private readonly IRecordsConfigurationValidator _configurationValidator;
        private static TDbContext? _context;

        private TRecords? _records;
        private ConstructorInfo _ctorOfImpl = null!;


        public Factory(
            IRecordHandlerFactory handlerFactory,
            IRecordsConfigurationValidator configurationValidator)
        {
            _handlerFactory = handlerFactory;
            _configurationValidator = configurationValidator;
        }

        private readonly Func<TDbContext> _getContext = () => _context ?? throw new ArgumentNullException("DbContext has not been provided");



        public object ConstructAppConfiguration(ConstructorInfo ctorOfimpl, TDbContext context, IServiceScopeFactory scopeFactory)
        {
            _ctorOfImpl = ctorOfimpl;
            ValidateCtorOfImplementation();

            _context = context;
            if (_records is null)
                SetUpRecords();

            return _ctorOfImpl.Invoke(new object[] { _getContext(), scopeFactory, _records! });
        }


        private static readonly Type[] _ctorParamTypes = new Type[] { typeof(TDbContext), typeof(IServiceScopeFactory), typeof(TRecords) };
        private void ValidateCtorOfImplementation()
        {
            if (_ctorOfImpl is null)
                throw new TypeLoadException("Library internal error, missing constructor");

            var ctorParams = _ctorOfImpl.GetParameters().Select(x => x.ParameterType).ToArray();

            if (_ctorParamTypes.Length != ctorParams.Length
                || !_ctorParamTypes.Zip(ctorParams).All(types => types.First == types.Second))
                throw new TypeLoadException("Library internal error, invalid constructor");
        }


        private void SetUpRecords()
        {
            _records = new TRecords();

            var setter = new RecordsPropertiesSetter<TDbContext>(_configurationValidator, _handlerFactory, _getContext);

            setter.SetUp(_records);
        }
    }
}
