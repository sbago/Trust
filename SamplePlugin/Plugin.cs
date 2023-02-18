using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Trust.Windows;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;

namespace Trust
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Trust";
        private const string CommandName = "/trust";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Trust");

        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }
        private delegate uint action(uint a1, long a2,long a3);
        private Hook<action> hook;
        public delegate long SetPostionDelegate(IntPtr objAddress, float x, float y, float z);
        public Hook<SetPostionDelegate> SetPostion;
        public delegate long GetPostionDelegate(IntPtr objAddress);
        public Hook<GetPostionDelegate> GetPostion;
        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            Dalamud.Initialize(pluginInterface);
            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this, goatImage);
            
            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);
            ConfigWindow.IsOpen =true;

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            hook = Hook<action>.FromAddress(Dalamud.SigScanner.ScanText("E8 ?? ?? ?? ?? 85 C0 75 02 33 C0"), detour);
            hook.Enable();
            //83 79 7C 00 75 09 F6 81 ?? ?? ?? ?? ?? 74 2A  get pos
            //E8 ?? ?? ?? ?? 66 83 8B ?? ?? ?? ?? ?? 33 D2  set pos


        }

        private uint detour(uint a1, long a2, long a3)
        {
            var ret = hook.Original(a1, a2, a3);
            return 0;
        }

        public void Dispose()
        {
            hook.Dispose();
            this.WindowSystem.RemoveAllWindows();
            
            ConfigWindow.Dispose();
            MainWindow.Dispose();
            
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
