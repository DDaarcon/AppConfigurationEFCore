using AppConfigurationEFCore.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using System.Reflection.Emit;

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

            if (_options.HasDefaultInterface)
                RegisterUserDefaultAppConfigurationFactory();

            RegisterAppConfigurationFactoryMethod();

        }


        private void RegisterHeAuxiliaryFactories()
        {
            var recordHandlerFactory = new RecordHandlerFactory(_options.ReferenceTypeHandlers, _options.VTTypeHandlers);
            var configurationValidator = new RecordsConfigurationValidator();
            var factoryType = FactoryGenericType;

            var factoryConstructor = factoryType.GetConstructor(new Type[] { typeof(IRecordHandlerFactory), typeof(IRecordsConfigurationValidator) });


            _services.AddSingleton(factoryType, factoryConstructor!.Invoke(new object[] { recordHandlerFactory, configurationValidator }));
        }


        private void RegisterAppConfigurationFactoryMethod()
        {
            var registeredInterface = BaseInterfaceGenericType;
            if (_options.HasDefaultInterface && _options.RegisterOnlyUserDefaultInterface)
                registeredInterface = _options.DefaultInterface;

            _services.TryAddScoped(registeredInterface, services =>
            {
                var factory = services.GetRequiredService(FactoryGenericType);

                var dbContext = services.GetRequiredService(_dbContextType);
                var serviceScopeFactory = services.GetRequiredService<IServiceScopeFactory>();

                /// <see cref="Factory{TDbContext, TRecords}.ConstructAppConfiguration(TDbContext, IServiceScopeFactory)"/>
                var constructMethod = factory.GetType().GetMethod("ConstructAppConfiguration")!;

                return constructMethod
                    .Invoke(factory, new[] { ImplementationCtor, dbContext, serviceScopeFactory })!;
            });

            if (_options.HasDefaultInterface && !_options.RegisterOnlyUserDefaultInterface)
                _services.TryAddScoped(_options.DefaultInterface!, services => services.GetRequiredService(BaseInterfaceGenericType));
        }

        private void RegisterUserDefaultAppConfigurationFactory()
        {
            Type baseImplGenericType = BaseImplGenericType;
            Type userInterface = _options.DefaultInterface!;
            Type baseInterface = BaseInterfaceGenericType;

            AssemblyName asmName = new AssemblyName(
                string.Format("{0}_{1}", "tmpAsm", Guid.NewGuid().ToString("N"))
            );
            AssemblyBuilder asmBuilder =
                AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder =
                asmBuilder.DefineDynamicModule("core");
            string proxyTypeName = string.Format("{0}_{1}", userInterface.Name, Guid.NewGuid().ToString("N"));


            TypeBuilder typeBuilder =
                moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Class | TypeAttributes.Public, baseImplGenericType);

            typeBuilder.AddInterfaceImplementation(userInterface);
            typeBuilder.AddInterfaceImplementation(baseInterface);

            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                CtorOfImplParamTypes);

            var baseCtor = BaseImplGenericType.GetConstructor(CtorOfImplParamTypes);
            //var baseNonGenericCtor = BaseImplType.GetConstructor(BindingFlags.Public, null, CtorOfImplParamTypes, null);
            //var baseCtor = TypeBuilder.GetConstructor(BaseImplGenericType, baseNonGenericCtor!);

            var generator = ctorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Ldarg_1); // TDbContext
            generator.Emit(OpCodes.Ldarg_2); // IServiceScopeFactory
            generator.Emit(OpCodes.Ldarg_3); // TRecords
            generator.Emit(OpCodes.Call, baseCtor!);

            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ret);


            _userImplementationType = typeBuilder.CreateType();
        }


        private ConstructorInfo ImplementationCtor => _userImplementationType?.GetConstructor(CtorOfImplParamTypes)
            ?? BaseImplGenericType.GetConstructor(CtorOfImplParamTypes)!;

        private Type? _userImplementationType;

        private Type BaseImplGenericType => BaseImplType
                .MakeGenericType(_dbContextType, _configurationRecordsType)!;

        private Type BaseImplType => typeof(AppConfiguration<,>);

        private Type BaseInterfaceGenericType => typeof(IAppConfiguration<>).MakeGenericType(_configurationRecordsType);
        private Type FactoryGenericType => typeof(Factory<,>).MakeGenericType(_dbContextType, _configurationRecordsType);



        private Type[] CtorOfImplParamTypes => new Type[] { _dbContextType, typeof(IServiceScopeFactory), _configurationRecordsType };
    }
}
