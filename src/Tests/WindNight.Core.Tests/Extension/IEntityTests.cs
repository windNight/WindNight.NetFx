using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests.Extension
{
    public class IEntityTests : TestBase
    {
        public IEntityTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void GenDefaultTableNameTest()
        {
            var name = "testts";
            var t = new TestT();
            var dn1 = t.GenDefaultTableName(appendPlural: true);
            var dn2 = t.GenDefaultTableName<TestT>(appendPlural: true);
            Assert.Equal(name, dn1);
            Assert.Equal(name, dn2);
            Output($"dn1   Is {dn1} dn2 is {dn2}");
        }



        [Fact]
        public void ToTest()
        {
            var t1 = new TestT2();
            var t2 = t1.To<TestT2, TestT3>(converter: (_1, _2) =>
            {
                _2.T213 = _1.T21;
                _2.T223 = _1.T22;
            });

            Assert.Equal(t2.T213, t1.T21);
            Assert.Equal(t2.T223, t1.T22);
            Output($"t1   Is {t1.ToJsonStr()} t2 is {t2.ToJsonStr()}");
        }

    }

    public class TestT : IEntity
    {
        public string T1 { get; set; } = "T1";
        public string T2 { get; set; } = "T2";
        public string T3 { get; set; } = "T3";

    }

    public class TestT2 : TestT
    {
        public string T21 { get; set; } = "T21";
        public string T22 { get; set; } = "T22";


    }


    public class TestT3 : TestT
    {
        public string T213 { get; set; }
        public string T223 { get; set; }
        public string T31 { get; set; } = "T31";
        public string T32 { get; set; } = "T32";




    }
}
