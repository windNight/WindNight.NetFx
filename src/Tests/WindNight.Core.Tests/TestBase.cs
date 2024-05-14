using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace WindNight.Core.Tests
{
    public class TestBase
    {
        protected readonly ITestOutputHelper OutputHelper;

        public TestBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
            // Debug.SetProvider();

        }

        protected void Output(string message)
        {
            Console.WriteLine($"Console:{HardInfo.Now:yyyy-MM-dd HH:mm:sss}  {message}");
            OutputHelper.WriteLine($"ITestOutputHelper:{HardInfo.Now:yyyy-MM-dd HH:mm:sss}  {message}");
        }

    }


}
