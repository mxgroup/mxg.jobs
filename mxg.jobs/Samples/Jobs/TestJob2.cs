using Mxg.Jobs;
using Samples.Logic;

namespace Samples.Jobs
{
    public class TestJob2 : SingleCallCronJob
    {
        private readonly TestLogic _testLogic;

        public TestJob2(TestLogic testLogic)
        {
            _testLogic = testLogic;
        }

        public override string CronExpression => CronHelper.Seconds(1, 3);

        public override void Execute()
        {
            _testLogic.DoTestWork2();
        }
    }
}