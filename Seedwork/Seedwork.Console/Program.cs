using Seedwork.Console.Sandboxes;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var scope = await IocContainerBuilder.SetupScope();
    }
}