﻿namespace Server;

using System.Net;
using System.Reflection;

using NLog;


public class Server
{
    public static int capacity = new Random().Next(0, 100);
    public static int lowerBound = 0;
    public static int upperBound = 0;

    /// <summary>
    /// Logger for this class.
    /// </summary>
    Logger log = LogManager.GetCurrentClassLogger();

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
    /// Program entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
        var self = new Server();
        self.Run(args);
    }

    /// <summary>
    /// Program body.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private void Run(string[] args)
    {
        //configure logging
        ConfigureLogging();

        //indicate server is about to start
        log.Info("Server is about to start");

        //start the server
        StartServer(args);

        while (true)
        {
            lowerBound = new Random().Next(0, 50);
            upperBound = new Random().Next(lowerBound + 1, 100);
            log.Info("Bounds changed to: " + lowerBound + " " + upperBound);
            Thread.Sleep(4000);
        }
    }

    /// <summary>
    /// Starts integrated server.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private void StartServer(string[] args)
    {
        //create web app builder
        var builder = WebApplication.CreateBuilder(args);

        //configure integrated server
        builder.WebHost.ConfigureKestrel(opts =>
        {
            opts.Listen(IPAddress.Loopback, 5000);
        });

        //add and configure swagger documentation generator (http://127.0.0.1:5000/swagger/)
        builder.Services.AddSwaggerGen(opts =>
        {
            //include code comments in swagger documentation
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        //turn on support for web api controllers
        builder.Services.AddControllers();

        //add CORS policies
        builder.Services.AddCors(cr =>
        {
            //allow everything from everywhere
            cr.AddPolicy("allowAll", cp =>
            {
                cp.AllowAnyOrigin();
                cp.AllowAnyMethod();
                cp.AllowAnyHeader();
            });
        });

        //build the server
        var app = builder.Build();

        //turn CORS policy on
        app.UseCors("allowAll");

        //turn on support for swagger doc web page
        app.UseSwagger();
        app.UseSwaggerUI();

        //turn on request routing
        app.UseRouting();

        //configure routes
        app.UseEndpoints(ep =>
        {
            ep.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}"
            );
        });

        //run the server
        //app.Run();
        app.RunAsync(); //use this if you need to implement background processing in the main thread
    }
}
