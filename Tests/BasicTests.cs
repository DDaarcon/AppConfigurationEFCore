using AppConfigurationEFCore;
using AppConfigurationEFCore.Setup;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Tests.Help.Db;

namespace Tests
{
    public class AppConfigurationEFCoreTests
    {
        private IAppConfiguration<BasicAppConfigRecords> _configuration = null!;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            TestDatabase.CreateContext(TestServices.Collection);

            TestServices.Collection.AddAppConfiguration<MyDbContext, BasicAppConfigRecords>();
        }

        [SetUp]
        public void Setup()
        {
            _configuration = TestServices.GetService<IAppConfiguration<BasicAppConfigRecords>>();
        }


        [Test]
        public async Task Should_SetAndSaveAsync_and_GetAsync_string()
        {
            Assert.IsNull(await _configuration.Records.Name.GetAsync());

            string txt = "some name";
            await _configuration.Records.Name.SetAndSaveAsync(txt);

            Assert.AreEqual(txt, await _configuration.Records.Name.GetAsync());

            await _configuration.Records.Name.SetAndSaveAsync(null);
        }

        [Test]
        public async Task Should_SetAndSaveAsync_and_GetAsync_int()
        {
            Assert.IsNull(await _configuration.Records.SomeNumber.GetAsync());

            int num = 45;
            await _configuration.Records.SomeNumber.SetAndSaveAsync(num);

            Assert.AreEqual(num, await _configuration.Records.SomeNumber.GetAsync());

            await _configuration.Records.SomeNumber.SetAndSaveAsync(null);
        }

        [Test]
        public async Task Should_SetAndSaveAsync_and_GetAsync_decimal()
        {
            Assert.IsNull(await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            decimal num = 213.542m;
            await _configuration.Records.SomeFloatingPointNumber.SetAndSaveAsync(num);

            Assert.AreEqual(num, await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            await _configuration.Records.SomeFloatingPointNumber.SetAndSaveAsync(null);
        }




        [Test]
        public void Should_SetAndSave_and_Get_string()
        {
            Assert.IsNull(_configuration.Records.Name.Get());

            string txt = "some name";
            _configuration.Records.Name.SetAndSave(txt);

            Assert.AreEqual(txt, _configuration.Records.Name.Get());

            _configuration.Records.Name.SetAndSave(null);
        }

        [Test]
        public void Should_SetAndSave_and_Get_int()
        {
            Assert.IsNull(_configuration.Records.SomeNumber.Get());

            int num = 45;
            _configuration.Records.SomeNumber.SetAndSave(num);

            Assert.AreEqual(num, _configuration.Records.SomeNumber.Get());

            _configuration.Records.SomeNumber.SetAndSave(null);
        }

        [Test]
        public void Should_SetAndSave_and_Get_decimal()
        {
            Assert.IsNull(_configuration.Records.SomeFloatingPointNumber.Get());

            decimal num = 213.542m;
            _configuration.Records.SomeFloatingPointNumber.SetAndSave(num);

            Assert.AreEqual(num, _configuration.Records.SomeFloatingPointNumber.Get());

            _configuration.Records.SomeFloatingPointNumber.SetAndSave(null);
        }










        [Test]
        public async Task Should_Set_SaveAsync_and_GetAsync_string()
        {
            Assert.IsNull(await _configuration.Records.Name.GetAsync());

            string txt = "some name";
            await _configuration.Records.Name.SetAsync(txt);
            await _configuration.SaveAsync();

            Assert.AreEqual(txt, await _configuration.Records.Name.GetAsync());

            await _configuration.Records.Name.SetAndSaveAsync(null);
        }

        [Test]
        public async Task Should_Set_SaveAsync_and_GetAsync_int()
        {
            Assert.IsNull(await _configuration.Records.SomeNumber.GetAsync());

            int num = 45;
            await _configuration.Records.SomeNumber.SetAsync(num);
            await _configuration.SaveAsync();

            Assert.AreEqual(num, await _configuration.Records.SomeNumber.GetAsync());

            await _configuration.Records.SomeNumber.SetAndSaveAsync(null);
        }

        [Test]
        public async Task Should_Set_SaveAsync_and_GetAsync_decimal()
        {
            Assert.IsNull(await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            decimal num = 213.542m;
            await _configuration.Records.SomeFloatingPointNumber.SetAsync(num);
            await _configuration.SaveAsync();

            Assert.AreEqual(num, await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            await _configuration.Records.SomeFloatingPointNumber.SetAndSaveAsync(null);
        }




        [Test]
        public void Should_Set_Save_and_Get_string()
        {
            Assert.IsNull(_configuration.Records.Name.Get());

            string txt = "some name";
            _configuration.Records.Name.Set(txt);
            _configuration.Save();

            Assert.AreEqual(txt, _configuration.Records.Name.Get());

            _configuration.Records.Name.SetAndSave(null);
        }

        [Test]
        public void Should_Set_Save_and_Get_int()
        {
            Assert.IsNull(_configuration.Records.SomeNumber.Get());

            int num = 45;
            _configuration.Records.SomeNumber.Set(num);
            _configuration.Save();

            Assert.AreEqual(num, _configuration.Records.SomeNumber.Get());

            _configuration.Records.SomeNumber.SetAndSave(null);
        }

        [Test]
        public void Should_Set_Save_and_Get_decimal()
        {
            Assert.IsNull(_configuration.Records.SomeFloatingPointNumber.Get());

            decimal num = 213.542m;
            _configuration.Records.SomeFloatingPointNumber.Set(num);
            _configuration.Save();

            Assert.AreEqual(num, _configuration.Records.SomeFloatingPointNumber.Get());

            _configuration.Records.SomeFloatingPointNumber.SetAndSave(null);
        }










        [Test]
        public async Task Should_SetIfEmptyAsync_SaveAsync_and_GetAsync_string()
        {
            Assert.IsNull(await _configuration.Records.Name.GetAsync());

            string txt = "some name";
            await _configuration.Records.Name.SetIfEmptyAsync(txt);
            await _configuration.SaveAsync();

            Assert.AreEqual(txt, await _configuration.Records.Name.GetAsync());

            await _configuration.Records.Name.SetIfEmptyAsync("dsadadasadd");
            await _configuration.SaveAsync();

            Assert.AreEqual(txt, await _configuration.Records.Name.GetAsync());

            await _configuration.Records.Name.SetAndSaveAsync(null);
        }

        [Test]
        public async Task Should_SetIfEmptyAsync_SaveAsync_and_GetAsync_int()
        {
            Assert.IsNull(await _configuration.Records.SomeNumber.GetAsync());

            int num = 45;
            await _configuration.Records.SomeNumber.SetIfEmptyAsync(num);
            await _configuration.SaveAsync();

            Assert.AreEqual(num, await _configuration.Records.SomeNumber.GetAsync());

            await _configuration.Records.SomeNumber.SetIfEmptyAsync(5234);
            await _configuration.SaveAsync();

            Assert.AreEqual(num, await _configuration.Records.SomeNumber.GetAsync());

            await _configuration.Records.SomeNumber.SetAndSaveAsync(null);
        }

        [Test]
        public async Task Should_SetIfEmptyAsync_SaveAsync_and_GetAsync_decimal()
        {
            Assert.IsNull(await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            decimal num = 213.542m;
            await _configuration.Records.SomeFloatingPointNumber.SetIfEmptyAsync(num);
            await _configuration.SaveAsync();

            Assert.AreEqual(num, await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            await _configuration.Records.SomeFloatingPointNumber.SetIfEmptyAsync(312.431m);
            await _configuration.SaveAsync();

            Assert.AreEqual(num, await _configuration.Records.SomeFloatingPointNumber.GetAsync());

            await _configuration.Records.SomeFloatingPointNumber.SetAndSaveAsync(null);
        }




        [Test]
        public void Should_SetIfEmpty_Save_and_Get_string()
        {
            Assert.IsNull(_configuration.Records.Name.Get());

            string txt = "some name";
            _configuration.Records.Name.SetIfEmpty(txt);
            _configuration.Save();

            Assert.AreEqual(txt, _configuration.Records.Name.Get());

            _configuration.Records.Name.SetIfEmpty("dasdaSDAD");
            _configuration.Save();

            Assert.AreEqual(txt, _configuration.Records.Name.Get());

            _configuration.Records.Name.SetAndSave(null);
        }

        [Test]
        public void Should_SetIfEmpty_Save_and_Get_int()
        {
            Assert.IsNull(_configuration.Records.SomeNumber.Get());

            int num = 45;
            _configuration.Records.SomeNumber.SetIfEmpty(num);
            _configuration.Save();

            Assert.AreEqual(num, _configuration.Records.SomeNumber.Get());

            _configuration.Records.SomeNumber.SetIfEmpty(4324);
            _configuration.Save();

            Assert.AreEqual(num, _configuration.Records.SomeNumber.Get());

            _configuration.Records.SomeNumber.SetAndSave(null);
        }

        [Test]
        public void Should_SetIfEmpty_Save_and_Get_decimal()
        {
            Assert.IsNull(_configuration.Records.SomeFloatingPointNumber.Get());

            decimal num = 213.542m;
            _configuration.Records.SomeFloatingPointNumber.SetIfEmpty(num);
            _configuration.Save();

            Assert.AreEqual(num, _configuration.Records.SomeFloatingPointNumber.Get());

            _configuration.Records.SomeFloatingPointNumber.SetIfEmpty(432.54m);
            _configuration.Save();

            Assert.AreEqual(num, _configuration.Records.SomeFloatingPointNumber.Get());

            _configuration.Records.SomeFloatingPointNumber.SetAndSave(null);
        }
    }
}
