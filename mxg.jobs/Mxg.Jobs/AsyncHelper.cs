using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mxg.Jobs
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            MyTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Fire and Forget. Убирает предупреждение компилятора warning CS4014.
        /// </summary>
        /// <param name="task">Исходный таск.</param>
        /// <remarks>Взято отсюда http://stackoverflow.com/questions/22629951/suppressing-warning-cs4014-because-this-call-is-not-awaited-execution-of-the .</remarks>
        public static void Forget(this Task task)
        {
        }
    }
}
