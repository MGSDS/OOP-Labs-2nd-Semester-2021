using System;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public static class Prompts
    {
        internal static void ReturnPrompt()
        {
            AnsiConsole.Write(new Text("Press Enter to return back"));
            Console.Read();
        }

        internal static Guid AskGuid(string text)
        {
            return AnsiConsole.Ask<Guid>($"Enter [green]{text}[/]?");
        }
    }
}