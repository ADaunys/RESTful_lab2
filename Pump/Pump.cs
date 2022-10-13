namespace Client;

using System.Net.Http;

using NLog;

using ServiceReference;


/// <summary>
/// Pump
/// </summary>
class Pump
{
    /// <summary>
    /// Logger for this class.
    /// </summary>
    private Logger log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Configure loggin subsystem.
    /// </summary>
    private void ConfigureLogging()
    {
        var config = new NLog.Config.LoggingConfiguration();

        var console =
            new NLog.Targets.ConsoleTarget("console")
            {
                Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
            };
        config.AddTarget(console);
        config.AddRuleForAllLevels(console);

        LogManager.Configuration = config;
    }

    /// <summary>
    /// Program body.
    /// </summary>
    private void Run()
    {
        ConfigureLogging();

        //main loop
        while (true)
        {
            try
            {
                //connect to server
                var service = new PumpService("http://127.0.0.1:5000", new HttpClient());

                //test service
                var rnd = new Random();

                while (true)
                {
                    //test simple call
                    var left = rnd.Next(-100, 100);
                    var right = rnd.Next(-100, 100);

                    log.Info($"Before 'int Add(int, int)': left={left}, right={right}");
                    var sum = service.AddLiteral(left, right);
                    log.Info($"After 'int Add(int, int)': sum={sum}, left={left}, right={right}\n");

                    left = rnd.Next(-100, 100);
                    right = rnd.Next(-100, 100);

                    Thread.Sleep(2000);

                    Thread.Sleep(2000);
                }
            }
            catch (Exception e)
            {
                //log exceptions
                log.Error(e, "Unhandled exception caught. Restarting.");

                //prevent console spamming
                Thread.Sleep(2000);
            }
        }
    }

    /// <summary>
    /// Program entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    static void Main(string[] args)
    {
        var self = new Pump();
        self.Run();
    }
}