//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using DbUp;
using Microsoft.Extensions.Configuration;

/// <summary>
/// The main program class responsible for running database migrations and seeding the database.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point for applying database migrations and seeding the database.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>An integer representing the success or failure of the operation.</returns>
    public static int Main(string[] args)
    {
        string migrationPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Streetcode.DAL",
            "Persistence",
            "ScriptsMigration");

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Streetcode.WebApi"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("STREETCODE_")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        string? pathToScript = null;

        Console.WriteLine("Enter '-m' to MIGRATE or '-s' to SEED db:");
        pathToScript = Console.ReadLine();

        pathToScript = migrationPath;
        var upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(pathToScript)
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
#if DEBUG
            Console.ReadLine();
#endif
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        return 0;
    }
}