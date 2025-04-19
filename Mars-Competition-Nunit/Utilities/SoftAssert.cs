using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Competition_Nunit.Utilities
{
    class SoftAssert
    {
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                // Log the error without throwing an exception
                TestContext.Out.WriteLine($"[SoftAssert Failure] {message}");
                Console.WriteLine($"[SoftAssert Failure] {message}");
            }
        }
    }
}
