using tracker.Enums;
using tracker.Models;
using tracker.Interfaces;
using tracker.Services;
using tracker.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();
var _ninmuService = serviceProvider.GetService<INinmuServices>();

DisplayWelcomeMessage();
List<string> commands = [];

while (true) {
    Utility.PrintCommandMessage("Enter command: ");
    string input = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrEmpty(input)) {
        Utility.PrintInfoMessage("\n No input detected. Please try again.");
        continue;
    }

    commands = Utility.ParseInput(input);
    string firstCommand = commands[0].ToLower();
    bool yameruka = false;

    switch (firstCommand) {
        case "add":
            NinmuTennka();
            break;

        case "update":
            NinmuKoushinn();
            break;

        case "delete":
            NinmuSakujyou();
            break;

        case "mark-todo":
            SetSatusOfNinmu();
            break;

        case "mark-progressing":
            SetSatusOfNinmu();
            break;

        case "mark-done":
            SetSatusOfNinmu();
            break;

        case "list":
            ListNinmus();
            break;
            
        case "help":
            DisplayAllCommands();
            break;

        case "exit":
            yameruka = true;
            break;

        case "clear":
            Utility.ClearConsole();
            DisplayWelcomeMessage();
            continue;

        default:
            break;  
    }

    if (yameruka)
        break;
}

void NinmuTennka() {
    if (!IsInputValid(commands, 2))
        return;

    var addedID = _ninmuService?.addNinmu(commands[1]);

    if (addedID != null && addedID.Result != 0)
        Utility.PrintInfoMessage($"Ninmu added successfully (ID: {addedID.Result})");
    else 
        Utility.PrintInfoMessage("Failed at adding.");
}

void NinmuKoushinn() {
    if (!IsInputValid(commands, 3))
        return;

    if (int.TryParse(commands[1], out int id)) {
        var dekitaka = _ninmuService?.updateNinmu(id, commands[2]).Result;

        if (dekitaka != null && dekitaka.Value)
            Utility.PrintInfoMessage($"Ninmu updated successfully (ID: {id})");
        else
            Utility.PrintInfoMessage($"ID: {id}, does not exist.");
            
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void NinmuSakujyou() {
    if (!IsInputValid(commands, 2))
        return;

    if (int.TryParse(commands[1], out int id)) {
        var dekitaka = _ninmuService?.deleteNinmu(id).Result;

        if (dekitaka != null && dekitaka.Value)
            Utility.PrintInfoMessage($"Ninmu deleted successfully (ID: {id})");
        else
            Utility.PrintInfoMessage($"ID: {id}, does not exist.");
            
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void SetSatusOfNinmu() {
    if (!IsInputValid(commands, 2))
        return;

    if (int.TryParse(commands[1], out int id)) {
        var dekitaka = _ninmuService?.statusSetti(id, commands[0]).Result;

        if (dekitaka != null && dekitaka.Value)
            Utility.PrintInfoMessage($"Ninmu status set successfully (ID: {id})");
        else
            Utility.PrintInfoMessage($"ID: {id}, does not exist.");

    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void ListNinmus() {
    if (commands.Count > 2) {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }

    var risuto = new List<Ninmu>();

    if (commands.Count == 1)
        risuto = _ninmuService?.getAllNinmu().Result.OrderBy(n => n.ID).ToList() ?? [];
    else {
        if (!commands[1].ToLower().Equals("todo") && !commands[1].ToLower().Equals("progressing") && !commands[1].ToLower().Equals("done")) {
            Utility.PrintErrorMessage("Wrong command. Please try again.");
            Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
            return;
        }

        risuto = _ninmuService?.getNinmuByStatus(commands[1]).Result.OrderBy(n => n.ID).ToList() ?? [];
    }

    DisplayNinmuAsTable(risuto);
}

void DisplayAllCommands() {
    var commandRisuto = _ninmuService?.getAllCommands();
    int count = 1;

    if (commandRisuto != null)
        foreach (var i in commandRisuto) {
            Utility.PrintHelpMessage(count + ". " + i);
            count++;
        }
}

#region helper methods

static void ConfigureServices(IServiceCollection i) {
    i.AddSingleton<INinmuServices, NinmuServices>();
} 

static void DisplayWelcomeMessage() {
    Utility.PrintInfoMessage("Hi, welcome to Ninmu Tracker.");
    Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
}

static bool IsInputValid(List<string> commands, int requiredQuantity) {
    bool ifValid = true;

    if (requiredQuantity == 1)
        if (commands.Count != 1)
            ifValid = false;

    if (requiredQuantity == 2)
        if (commands.Count != 2 || string.IsNullOrEmpty(commands[1]))
            ifValid = false;

    if (requiredQuantity == 3)
        if (commands.Count != 3 || string.IsNullOrEmpty(commands[1]) || string.IsNullOrEmpty(commands[2]))
            ifValid = false;

    if (!ifValid) {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return false;
    }

    return true;
}

static void DisplayNinmuAsTable(List<Ninmu> risuto) {
    int colWidth1 = 15, colWidth2 = 35, colWidth3 = 15, colWidth4 = 15, colWidth5 = 15;
    if (risuto != null && risuto.Count > 0) {
        Console.WriteLine("\n{0,-" + colWidth1 + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "} {4,-" + colWidth5 + "}",
            "Ninmu ID", "Description", "Status", "Created Date", "Updated Date" + "\n");

        foreach (var i in risuto) {
            SetConsoleTextColor(i);
            Console.WriteLine("{0,-" + colWidth1 + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "} {4,-" + colWidth5 + "}"
                , i.ID, i.description, i.ninmuJyoutai, i.createdAt.Date.ToString("dd-MM-yyyy"), i.updatedAt.Date.ToString("dd-MM-yyyy"));
            Console.ResetColor();
        }

    } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n No Ninmu exists! \n");
        Console.ResetColor();

        Console.WriteLine("{0,-" + colWidth1 + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "} {4,-" + colWidth5 + "}",
           "Ninmu ID", "Description", "Status", "Created Date", "Updated Date");
    }
}

static void SetConsoleTextColor(Ninmu i) {
    if (i.ninmuJyoutai == Status.todo)
        Console.ForegroundColor = ConsoleColor.Magenta;   
    else if (i.ninmuJyoutai == Status.done)
        Console.ForegroundColor = ConsoleColor.Green;
    else
        Console.ForegroundColor = ConsoleColor.Yellow;
}
#endregion