using AppConfigurationEFCore;
using AppConfigurationEFCore.Configuration;
using AppConfigurationEFCore.Entities;
using AppConfigurationEFCore.Setup;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tests.Help.Db;

namespace Tests
{
    internal class ConfigurationTests
    {
        [SetUp]
        public void SetupOnce()
        {
            TestServices.Clear();
            TestDatabase.DisposeContext();
            TestDatabase.CreateContext(TestServices.Collection);
        }

        [Test]
        public void Should_throw_error_invalid_records_type_invalid_property_type()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords>();
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
            public VTRecordHandler<int> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_not_registered_record_type()
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
            public VTRecordHandler<float> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_duplicate_keys()
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
            [RecordKey("aaa")]
            public VTRecordHandler<float> Bbb { get; set; } = null!;

        }

        [Test]
        public void Should_throw_error_invalid_records_type_vt_in_RecordHandler()
        {
            try
            {
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords5>();
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
                TestServices.Collection.AddAppConfiguration<MyDbContext, InvalidConfigRecords6>();
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

        [Test]
        public void Should_work_custom_ref_type()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords1>(options =>
            {
                options.Add<SimpleClass>(db =>
                {
                    var splitted = db?.Split(' ', 2);
                    if (splitted?.Length != 2) return null;
                    if (!int.TryParse(splitted[1], out int value)) return null;
                    return new SimpleClass
                    {
                        Name = splitted[0],
                        Number = value
                    };
                }, en => en is not null ? $"{en.Name} {en.Number}" : null);
            });
            TestServices.GetService<IAppConfiguration<ValidConfigRecords1>>();
        }
        private class ValidConfigRecords1
        {
            [RecordKey("aaa")]
            public RecordHandler<SimpleClass> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;

        }
        private class SimpleClass
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }


        [Test]
        public void Should_work_custom_value_type()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords2>(options =>
            {
                options.AddVT<char>(db => db?.FirstOrDefault(), en => en is not null ? $"{en}" : null);
            });
            TestServices.GetService<IAppConfiguration<ValidConfigRecords2>>();
        }
        private class ValidConfigRecords2
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<char> Bbb { get; set; } = null!;
        }

        [Test]
        public void Should_work_custom_ref_type_by_interface()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords3>(options =>
            {
                options.Add(new SimpleClass2Handler());
            });
            TestServices.GetService<IAppConfiguration<ValidConfigRecords3>>();
        }
        private class ValidConfigRecords3
        {
            [RecordKey("aaa")]
            public RecordHandler<SimpleClass2> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;

        }
        private class SimpleClass2
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }
        private class SimpleClass2Handler : IRecordHandlerRule<SimpleClass2>
        {
            public string? FromType(SimpleClass2? en) => en is not null ? $"{en.Name} {en.Number}" : null;

            public SimpleClass2? ToType(string? db)
            {
                var splitted = db?.Split(' ', 2);
                if (splitted?.Length != 2) return null;
                if (!int.TryParse(splitted[1], out int value)) return null;
                return new SimpleClass2
                {
                    Name = splitted[0],
                    Number = value
                };
            }
        }


        [Test]
        public void Should_work_custom_value_type_by_interface()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords4>(options =>
            {
                options.AddVT(new CharHandler());
            });
            TestServices.GetService<IAppConfiguration<ValidConfigRecords4>>();
        }
        private class ValidConfigRecords4
        {
            [RecordKey("aaa")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<char> Bbb { get; set; } = null!;
        }
        private class CharHandler : IVTRecordHandlerRule<char>
        {
            public string? FromType(char? en) => en is not null ? $"{en}" : null;

            public char? ToType(string? db) => db?.FirstOrDefault();
        }


        [Test]
        public async Task Should_trim_record_key()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords5>();
            var svc = TestServices.GetService<IAppConfiguration<ValidConfigRecords5>>();

            await svc.Records.Aaa.SetAndSaveAsync("abc");
            Assert.AreEqual("abc", await svc.CustomConfig("aaa").GetAsync());
        }
        private class ValidConfigRecords5
        {
            [RecordKey("  aaa  ")]
            public RecordHandler<string> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;
        }




        [Test]
        public async Task Should_work_custom_ref_type_with_json_handler()
        {
            TestServices.Collection.AddAppConfiguration<MyDbContext, ValidConfigRecords6>(options =>
            {
                options.Add(new JsonRecordHandlerRule<SimpleClass3>());
            });
            var svc = TestServices.GetService<IAppConfiguration<ValidConfigRecords6>>();

            var obj = new SimpleClass3
            {
                Name = "abcd",
                Number = 1234
            };

            await svc.Records.Aaa.SetAndSaveAsync(obj);
            var objS = await svc.Records.Aaa.GetAsync();

            Assert.IsNotNull(objS);
            Assert.AreEqual(obj.Name, objS!.Name);
            Assert.AreEqual(obj.Number, objS.Number);
        }
        private class ValidConfigRecords6
        {
            [RecordKey("aaa")]
            public RecordHandler<SimpleClass3> Aaa { get; set; } = null!;
            [RecordKey("bbb")]
            public VTRecordHandler<int> Bbb { get; set; } = null!;

        }
        private class SimpleClass3
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

    }
}
