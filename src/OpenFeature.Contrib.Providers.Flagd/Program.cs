using System;

namespace OpenFeature.Contrib.Providers.Flagd
{
    class Hello {
        static void Main(string[] args) {
            var fp = new FlagdProvider(new Uri("http://localhost:8013"));

            var val = fp.ResolveBooleanValue("myBoolFlag", false, null);

            System.Console.WriteLine(val.ToString());
        }
    }
}


