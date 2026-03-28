namespace MiniChatGPT.ChatConsole.Commands
{
    public class TopKCommand : IReplCommand
    {
        public string Name
        {
            get { return "/topK"; }
        }
        public string Description
        {
            get { return "Встановлює TopK для генерації"; }
        }

        public void Execute(string[] args, CommandExecutionContext context)
        {
            if(args.Length > 0 && int.TryParse(args[0], out int newTopK))
            {
                context.Options.TopK = newTopK;
                context.PrintMessage("\nЗначення TopK оновлено");

                context.PrintMessage(context.GetStatusInfo());
            }
            else
            {
                context.PrintMessage("Вказано некоректне число");
            }
        }
    }
}
