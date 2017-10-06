using Mxg.Jobs;
using Samples.Logic;

namespace Samples.Jobs
{
    public class TestJob1 : QuartzJob4
    {
        private readonly TestLogic _testLogic;

        public TestJob1(TestLogic testLogic)
        {
            _testLogic = testLogic;
        }

        public override string CronExpression => CronHelper.Seconds(1);

        public override void Execute()
        {
            _testLogic.DoTestWork1();
        }
    }
}
