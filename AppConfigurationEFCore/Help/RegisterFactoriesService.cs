using AppConfigurationEFCore.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AppConfigurationEFCore.Help
{
    internal class RegisterFactoriesService
    {
        private IServiceCollection _services = null!;
        private Type _dbContextType = null!;
        private Type _configurationRecordsType = null!;
        private CustomRecordTypeOptions _options = null!;

        public void Register(IServiceCollection services, Type dbContextType, Type configurationRecordsType, Action<CustomRecordTypeOptions>? customRecordTypesAction = null)
        {
            _services = services;
            _dbContextType = dbContextType;
            _configurationRecordsType = configurationRecordsType;

            _options = new CustomRecordTypeOptions();
            customRecordTypesAction?.Invoke(_options);

            RegisterHeAuxiliaryFactories();

            RegisterAppConfigurationFactoryMethod();
        }


        private void RegisterHeAuxiliaryFactories()
        {
            var recordHandlerFactory = new RecordHandlerFactory(_options.ReferenceTypeHandlers, _options.VTTypeHandlers);
            var configurationValidator = new RecordsConfigurationValidator();
            var factoryType = FactoryType;

            var factoryConstructor = factoryType.GetConstructor(new Type[] { typeof(IRecordHandlerFactory), typeof(IRecordsConfigurationValidator) });


            _services.AddSingleton(factoryType, factoryConstructor!.Invoke(new object[] { recordHandlerFactory, configurationValidator }));
        }


        private void RegisterAppConfigurationFactoryMethod()
        {
            _services.TryAddScoped(AppConfigurationType, services =>
            {
                var factory = services.GetRequiredService(FactoryType);

                var dbContext = services.GetRequiredService(_dbContextType);
                var serviceScopeFactory = services.GetRequiredService<IServiceScopeFactory>();

                /// <see cref="Factory{TDbContext, TRecords}.ConstructAppConfiguration(TDbContext, IServiceScopeFactory)"/>
                var constructMethod = factory.GetType().GetMethod("ConstructAppConfiguration")!;

                return constructMethod
                    .Invoke(factory, new[] { dbContext, serviceScopeFactory })!;
            });
        }

        private Type AppConfigurationType => typeof(IAppConfiguration<>).MakeGenericType(_configurationRecordsType);
        private Type FactoryType => typeof(Factory<,>).MakeGenericType(_dbContextType, _configurationRecordsType);
    }
}
