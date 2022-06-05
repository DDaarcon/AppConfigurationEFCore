using AppConfigurationEFCore;
using AppConfigurationEFCore.Configuration;
using AppConfigurationEFCore.Setup;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace Tests
{
    internal class ConfigurationTests
    {

        [SetUp]
        public void SetupOnce()
        {
            TestServices.Clear();
            TestDatabase.CreateContext(TestServices.Collection);
        }

        [Test]
        public void Should_throw_error_invalid_records_type_invalid_property_type()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<DbContext, InvalidConfigRecords>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public string Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_missing_RecordKeyAttribute()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<DbContext, InvalidConfigRecords2>();
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
            public VTRecordHandler<int> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_not_registered_record_type()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<DbContext, InvalidConfigRecords3>();
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
            public VTRecordHandler<float> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_duplicate_keys()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<DbContext, InvalidConfigRecords4>();
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
            [RecordKey("aaa")]
            public VTRecordHandler<float> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_vt_in_RecordHandler()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<DbContext, InvalidConfigRecords5>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords5>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords5
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public RecordHandler<int> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_empty_key()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<DbContext, InvalidConfigRecords6>();
                TestServices.GetService<IAppConfiguration<InvalidConfigRecords6>>();
                Assert.IsTrue(false);
            }
            catch (Exception)
            { }
        }
        private class InvalidConfigRecords6
        {
            [RecordKey("")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;

        }
    }
}
