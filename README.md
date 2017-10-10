# mxg.jobs

Создаём класс с логикой и расписанием джоба

```csharp
public class MyTestJob : SingleCallCronJob
{
    /// <inheritdoc />
    public override void Execute()
    {
        Debug.WriteLine("Test");
    }

    /// <inheritdoc />
    public override string CronExpression => "0 0/1 * 1/1 * ? *"; // Каждую минуту
}
```

Настраиваем приложение

```csharp
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        var app = new JobsApplication(
            new[] { typeof(MyTestJob) },
            type => (SingleCallCronJob)Activator.CreateInstance(type));
        app.Run();
    }
}
```
