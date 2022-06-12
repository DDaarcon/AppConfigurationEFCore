using AppConfigurationEFCore;
using AppConfigurationEFCore.Configuration;
using AppConfigurationEFCore.Entities;
using AppConfigurationEFCore.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Help.Db;

namespace Tests
{
    public class NestedConfigurationTests
    {
        [SetUp]
        public void SetupOnce()
        {
            TestServices.Clear();
            TestDatabase.DisposeContext();
            TestDatabase.CreateContext(TestServices.Collection);
        }


        [Test]
        public async Task ShouldCreateNestedRecords_WhenClassWithSubrecordsHasAttributeAndDefaultSeparator()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords1>();
            var svc = TestServices.GetService<IAppConfiguration<ValidConfigRecords1>>();

            await svc.Records.Inner.Aaa.SetAndSaveAsync("aaaVal");

            TestDatabase.Context.Set<AppConfig>().Any(x => x.Key == "inner.aaa");

            var saved = await svc.Records.Inner.Aaa.GetAsync();

            Assert.IsNotNull(saved);
            Assert.AreEqual("aaaVal", saved);
        }
        private class ValidConfigRecords1
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;

            public ValidSubConfigRecords1 Inner { get; private set; } = null!;
        }
        [RecordGroup(GroupKey = "inner")]
        private class ValidSubConfigRecords1
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }


        [Test]
        public async Task ShouldCreateNestedRecords_WhenClassWithSubrecordsHasAttributeAndCustomSeparator()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords2>();
            var svc = TestServices.GetService<IAppConfiguration<ValidConfigRecords2>>();

            await svc.Records.Inner.Aaa.SetAndSaveAsync("aaaVal");

            TestDatabase.Context.Set<AppConfig>().Any(x => x.Key == "inner((--aaa");

            var saved = await svc.Records.Inner.Aaa.GetAsync();

            Assert.IsNotNull(saved);
            Assert.AreEqual("aaaVal", saved);
        }
        private class ValidConfigRecords2
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;

            public ValidSubConfigRecords2 Inner { get; private set; } = null!;
        }
        [RecordGroup(GroupKey = "inner", Separator = "((--")]
        private class ValidSubConfigRecords2
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }

        [Test]
        public async Task ShouldCreateNestedRecords_WhenPropertyHasAttributeAndCustomSeparator()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords3>();
            var svc = TestServices.GetService<IAppConfiguration<ValidConfigRecords3>>();

            await svc.Records.Inner.Aaa.SetAndSaveAsync("aaaVal");

            TestDatabase.Context.Set<AppConfig>().Any(x => x.Key == "inner/&&/aaa");

            var saved = await svc.Records.Inner.Aaa.GetAsync();

            Assert.IsNotNull(saved);
            Assert.AreEqual("aaaVal", saved);
        }
        private class ValidConfigRecords3
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
            [RecordGroup(GroupKey = "inner", Separator = "/&&/")]
            public ValidSubConfigRecords3 Inner { get; private set; } = null!;
        }
        private class ValidSubConfigRecords3
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }

        [Test]
        public async Task ShouldCreateNestedRecordsWithPropertyAttrValues_WhenBothClassAndPropertyHaveAttribute()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords4>();
            var svc = TestServices.GetService<IAppConfiguration<ValidConfigRecords4>>();

            await svc.Records.Inner.Aaa.SetAndSaveAsync("aaaVal");

            TestDatabase.Context.Set<AppConfig>().Any(x => x.Key == "inner2..aaa");

            var saved = await svc.Records.Inner.Aaa.GetAsync();

            Assert.IsNotNull(saved);
            Assert.AreEqual("aaaVal", saved);
        }
        private class ValidConfigRecords4
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
            [RecordGroup(GroupKey = "inner2", Separator = "..")]
            public ValidSubConfigRecords4 Inner { get; private set; } = null!;
        }
        [RecordGroup(GroupKey = "inner", Separator = "/&&/")]
        private class ValidSubConfigRecords4
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }



        #region Fails

        [Test]
        public void ShouldFail_WhenMissingRecordGroupAtrr()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords1>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords1>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords1
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
            public InvalidSubConfigRecords1 Inner { get; private set; } = null!;
        }
        private class InvalidSubConfigRecords1
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }


        [Test]
        public void ShouldFail_WhenNamesOverlap()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords2>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords2>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords2
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("inner.bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
            public InvalidSubConfigRecords2 Inner { get; private set; } = null!;
        }
        [RecordGroup(GroupKey = "inner")]
        private class InvalidSubConfigRecords2
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }

        [Test]
        public void ShouldFail_WhenInvalidPropertyType()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords3>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords3>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords3
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
            public InvalidSubConfigRecords3 Inner { get; private set; } = null!;
        }
        [RecordGroup(GroupKey = "inner")]
        private class InvalidSubConfigRecords3
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            [RecordKey("bbb")]
            public int Bbb { get; set; }
        }

        [Test]
        public void ShouldFail_WhenMissingRecordKeyAttr()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords4>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords4>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords4
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
            public InvalidSubConfigRecords4 Inner { get; private set; } = null!;
        }
        [RecordGroup(GroupKey = "inner")]
        private class InvalidSubConfigRecords4
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;

            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }


        #endregion
    }
}
