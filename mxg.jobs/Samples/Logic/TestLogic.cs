using System;
using System.Diagnostics;
using System.Threading;

namespace Samples.Logic
{
    public class TestLogic
    {
        private static readonly Random Rnd = new Random();

        public void DoTestWork1()
        {
            const string name = nameof(DoTestWork1);
            Debug.WriteLine($"Start {DateTime.Now}:{name}");
            Thread.Sleep(TimeSpan.FromSeconds(Rnd.Next(5, 5)));
            Debug.WriteLine($"End {DateTime.Now}:{name}");
        }

        public void DoTestWork2()
        {
            const string name = nameof(DoTestWork2);
            Debug.WriteLine($"Start {DateTime.Now}:{name}");
            Thread.Sleep(TimeSpan.FromSeconds(Rnd.Next(5, 5)));
            Debug.WriteLine($"End {DateTime.Now}:{name}");
        }
    }
}
