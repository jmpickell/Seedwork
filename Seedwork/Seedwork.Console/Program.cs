﻿using Seedwork.Console.Sandboxes;
using Seedwork.Repositories.Interfaces;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var scope = await IocContainerBuilder.SetupScope();
        await DbSandbox.Run(scope, db: "MySQL");        
    }
}