using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Utility.Signatures;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Data;

namespace Trust
{
    internal class Dalamud
    {
        public static void Initialize(DalamudPluginInterface pluginInterface) =>
            pluginInterface.Create<Dalamud>();

        [PluginService]
        [RequiredVersion("1.0")]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        public static CommandManager Commands { get; private set; } = null!;
        [PluginService] public static GameNetwork GameNetwork { get; private set; }
        [PluginService] public static SigScanner SigScanner { get; private set; }
        [PluginService] public static ClientState ClientState { get; private set; }
        [PluginService] public static Condition Condition { get; private set; }
        [PluginService] public static GameGui GameGui { get; private set; }
        [PluginService] public static Framework Framework { get; private set; }
        [PluginService] public static ObjectTable ObjectTable { get; private set; }
        [PluginService] public static DataManager DataManager { get; private set; }
    }
}
