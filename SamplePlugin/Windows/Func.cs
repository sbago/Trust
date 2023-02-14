using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace Trust.Windows
{
    #region Enum
    public enum Version:uint
    {
        Shadowbringers = 0,
        Endwalker = 1
    }
    public enum CallBackTpye : int
    {
        SelectVersion = 20,
        SelectCharacter = 12,
        SelectDuty = 15,
        Start = 14
    }
    public enum Character : int
    {
        Alphinaud = 0,
        Alisaie = 1,
        Thancred = 2,
        Urianger = 3,
        Y_shtola = 4,
        RyneOrEstinien = 5,
        G_raha_Tia = 6
    }
    #endregion
    public unsafe static class Func
    {
        private static AgentInterface* Dawn => Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Dawn);
        private static AtkUnitBase* DawnAddon => (AtkUnitBase*) Dalamud.GameGui.GetAddonByName("Dawn", 1);
        private static AgentInterface* ContentsFinderMenu => Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinderMenu);
        private static AtkUnitBase* ContentsFinderMenuAddon => (AtkUnitBase*)Dalamud.GameGui.GetAddonByName("ContentsFinderMenu", 1);
        private static bool CanDoSomething => Dawn->IsAgentActive();
        public static void OpenOrCloseDawn(bool open = true)
        {
            if(open != Dawn->IsAgentActive())
            {
                Dawn->Show();
            }
        }
        public static void OpenDawn()
        {
            OpenOrCloseDawn();
        }
        public static void CloseDawn()
        {
            OpenOrCloseDawn(false);
        }
        public static void SelectVersion(Version version)
        {
            if(CanDoSomething)
                Callback(DawnAddon, (int)CallBackTpye.SelectVersion, (uint)version);
        }
        public static void SelectCharacter(Character character)
        {
            if (CanDoSomething)
                Callback(DawnAddon, (int)CallBackTpye.SelectCharacter, (uint)character);
        }
        //先选版本->副本-> 人
        public static void SelectDuty(uint duty)
        {
            if (CanDoSomething)
                Callback(DawnAddon, (int)CallBackTpye.SelectDuty, duty);
        }
        public static void Fire()
        {
            if (CanDoSomething)
                Callback(DawnAddon, (int)CallBackTpye.Start);
        }
        public static void SelectYesContentsFinderConfirm()
        {
            Callback((AtkUnitBase*)Dalamud.GameGui.GetAddonByName("ContentsFinderConfirm", 1), 8);
        }
        public static void ShowContentsFinderMenu()
        {
            ContentsFinderMenu->Show();
        }
        public static void LeaveDuty()
        {
            if (!ContentsFinderMenu->IsAgentActive())
                ShowContentsFinderMenu();
            Callback(ContentsFinderMenuAddon, 0);
            Callback((AtkUnitBase*)Dalamud.GameGui.GetAddonByName("SelectYesno", 1), 0);
        }
        private static void Callback(AtkUnitBase* unitBase, params object[] values)
        {
            if (unitBase == null) throw new Exception("Null UnitBase");
            var atkValues = (AtkValue*)Marshal.AllocHGlobal(values.Length * sizeof(AtkValue));
            if (atkValues == null) return;
            try
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var v = values[i];
                    switch (v)
                    {
                        case uint uintValue:
                            atkValues[i].Type = ValueType.UInt;
                            atkValues[i].UInt = uintValue;
                            break;
                        case int intValue:
                            atkValues[i].Type = ValueType.Int;
                            atkValues[i].Int = intValue;
                            break;
                        case float floatValue:
                            atkValues[i].Type = ValueType.Float;
                            atkValues[i].Float = floatValue;
                            break;
                        case bool boolValue:
                            atkValues[i].Type = ValueType.Bool;
                            atkValues[i].Byte = (byte)(boolValue ? 1 : 0);
                            break;
                        case string stringValue:
                            {
                                atkValues[i].Type = ValueType.String;
                                var stringBytes = Encoding.UTF8.GetBytes(stringValue);
                                var stringAlloc = Marshal.AllocHGlobal(stringBytes.Length + 1);
                                Marshal.Copy(stringBytes, 0, stringAlloc, stringBytes.Length);
                                Marshal.WriteByte(stringAlloc, stringBytes.Length, 0);
                                atkValues[i].String = (byte*)stringAlloc;
                                break;
                            }
                        default:
                            throw new ArgumentException($"Unable to convert type {v.GetType()} to AtkValue");
                    }
                }

                unitBase->FireCallback(values.Length, atkValues);
            }
            finally
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (atkValues[i].Type == ValueType.String)
                    {
                        Marshal.FreeHGlobal(new IntPtr(atkValues[i].String));
                    }
                }
                Marshal.FreeHGlobal(new IntPtr(atkValues));
            }
        }
    }
}
