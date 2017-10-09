using System.Windows;
using Mxg.Jobs;
using Samples.Jobs;
using SimpleInjector;

namespace Samples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Список типов
            var jobList = new[]
            {
                typeof(TestJob1),
                typeof(TestJob2)
            };

            // Способ создания
            var container = new Container();
            
            // Запуск
            var app = new JobsApplication(jobList, type => (SingleCallCronJob)container.GetInstance(type));
            app.Run();
        }
    }
}
