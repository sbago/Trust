using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    #region Struct
    public struct ToDolistInfo
    {

    }
    public unsafe struct DawnInfo
    {
        public Version Version;
        public uint Duty;
        public string DutyName;
        public CharacterInfo[] Characters = new CharacterInfo[4];

        public DawnInfo()
        {
        }

        public override string ToString()
        {
            var i =string.Empty;
            i +=  $"\n Version:{Version} DutyName:{DutyName}";
            for(var a =0;a<4;a++)
            {
                if(Characters[a].IsNull())
                {
                    PluginLog.Information("111111");
                    i += $"\n Characters{a} is null.";
                }
                else
                {
                    i += $"\n Characters:{Characters[a].Name} {Characters[a].JobName} {Characters[a].Lvl} {Characters[a].CurrentExp} IsMaxLvl:{Characters[a].IsMaxLvl}";
                }
            }
            return i;
        }
    }
    public struct CharacterInfo
    {
        public bool _IsNull;
        public string Name;
        public uint Lvl;
        public uint CurrentExp;
        public uint MaxExp;
        public string JobName;
        public bool IsMaxLvl;
        public bool IsNull()
        {
            return _IsNull;
        }
    }
    #endregion
    public unsafe static class Func
    {
        private static AgentInterface* Dawn => Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Dawn);
        private static AtkUnitBase* DawnAddon => (AtkUnitBase*)Dalamud.GameGui.GetAddonByName("Dawn", 1);
        private static AgentInterface* ContentsFinderMenu => Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinderMenu);
        private static AtkUnitBase* ContentsFinderMenuAddon => (AtkUnitBase*)Dalamud.GameGui.GetAddonByName("ContentsFinderMenu", 1);
        private static bool CanDoSomething => Dawn->IsAgentActive();
        public static void OpenOrCloseDawn(bool open = true)
        {
            if (open != Dawn->IsAgentActive())
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
            if (CanDoSomething)
                Callback(DawnAddon, (int)CallBackTpye.SelectVersion, (uint)version);
        }
        public static void SelectCharacter(Character character)
        {
            if (CanDoSomething)
                Callback(DawnAddon, (int)CallBackTpye.SelectCharacter, (uint)character);
        }
        //????????????->??????-> ???
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
        public static void AutoSelectYesContentsFinderConfirm()
        {
            Dalamud.Condition.ConditionChange += Condition_ConditionChange;
        }
        public static void RemovAutoSelectYesContentsFinderConfirmEvent()
        {
            Dalamud.Condition.ConditionChange -= Condition_ConditionChange;
        }

        private static void Condition_ConditionChange(global::Dalamud.Game.ClientState.Conditions.ConditionFlag flag, bool value)
        {
            if (flag == global::Dalamud.Game.ClientState.Conditions.ConditionFlag.WaitingForDutyFinder && value)
                SelectYesContentsFinderConfirm();
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
            if (Dalamud.Condition[global::Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty] &&
                Dalamud.Condition[global::Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty56] &&
                !Dalamud.Condition[global::Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
            {
                if (!ContentsFinderMenu->IsAgentActive())
                    ShowContentsFinderMenu();
                Callback(ContentsFinderMenuAddon, 0);
                Callback((AtkUnitBase*)Dalamud.GameGui.GetAddonByName("SelectYesno", 1), 0);
            }
        }
        #region SetPos
        private static IntPtr SetPosFunPtr
        {
            get
            {
                if (Dalamud.SigScanner.TryScanText("E8 ?? ?? ?? ?? 66 83 8B ?? ?? ?? ?? ?? 33 D2", out var ptr))
                    return ptr;
                return IntPtr.Zero;
            }
        }
        public static void SetPos(float x, float y, float z)
        {
            if (SetPosFunPtr == IntPtr.Zero)
                return;
            if (Dalamud.ClientState.LocalPlayer == null)
                return;
            ((delegate*<long, float, float, float, long>)SetPosFunPtr)((long)Dalamud.ClientState.LocalPlayer.Address, x, z, y);
        }
        public static void SetPos(float x, float y)
        {
            if (Dalamud.ClientState.LocalPlayer == null)
                return;
            float z = Dalamud.ClientState.LocalPlayer.Position.Y;
            SetPos(x, z, y);
        }
        public static void SetPos(Vector3 pos)
        {
            SetPos(pos.X, pos.Z, pos.Y);
        }
        public static void SetPos(Vector2 pos)
        {
            SetPos(pos.X, pos.Y);
        }
        //public static bool CanSetPosMore16M
        //{
        //    get
        //    {
        //        var TerritoryType = Dalamud.ClientState.TerritoryType;
        //        var a = Dalamud.DataManager.GetExcelSheet<TerritoryType>().GetRow(TerritoryType).Name.ToString();
        //    }
        //}
        public static void SetPosToMouse()
        {
            if (Dalamud.ClientState.LocalPlayer == null)
                return;
            var mousePos = ImGui.GetIO().MousePos;
            Dalamud.GameGui.ScreenToWorld(mousePos, out var pos);
            SetPos(pos);
        }
        #endregion
        #region UIinfo
        public static DawnInfo GetDawnInfo()
        {
            var DawnInfo = new DawnInfo();
            DawnInfo.Version = DawnAddon->UldManager.NodeList[97]->GetAsAtkComponentNode()->Component->UldManager.NodeList[3]->GetAsAtkNineGridNode()->AtkResNode.IsVisible ? Version.Shadowbringers : Version.Endwalker;
            for (uint i = 0; i < 4; i++)
            {
                if(DawnAddon->GetNodeById(98 + i)->GetAsAtkComponentNode()->Component->UldManager.NodeList[9]->Color.A == 0x00)
                    DawnInfo.Characters[i]._IsNull = true;
                DawnInfo.Characters[i].JobName = DawnAddon->GetNodeById(98 + i)->GetAsAtkComponentNode()->Component->UldManager.NodeList[11]->GetAsAtkTextNode()->NodeText.ToString();//??????
                DawnInfo.Characters[i].Name = DawnAddon->GetNodeById(98 + i)->GetAsAtkComponentNode()->Component->UldManager.NodeList[19]->GetAsAtkTextNode()->NodeText.ToString();//name
                DawnInfo.Characters[i].Lvl = uint.Parse(DawnAddon->GetNodeById(98 + i)->GetAsAtkComponentNode()->Component->UldManager.NodeList[22]->GetAsAtkComponentNode()->Component->UldManager.NodeList[0]->GetAsAtkCounterNode()->NodeText.ToString());//??????
                if (i==0)
                {
                    continue;
                }
                var CurrentExp = DawnAddon->GetNodeById(98 + i)->GetAsAtkComponentNode()->Component->UldManager.NodeList[15]->GetAsAtkTextNode()->NodeText.ToString();
                var MaxExp = DawnAddon->GetNodeById(98 + i)->GetAsAtkComponentNode()->Component->UldManager.NodeList[16]->GetAsAtkTextNode()->NodeText.ToString();
                if(MaxExp == "--")
                {
                    DawnInfo.Characters[i].IsMaxLvl= true;
                }
                else
                {
                    DawnInfo.Characters[i].CurrentExp = uint.Parse(CurrentExp.Replace(",", ""));
                    DawnInfo.Characters[i].MaxExp = uint.Parse(MaxExp.Replace(",", ""));
                }
            }
            DawnInfo.DutyName = DawnAddon->UldManager.NodeList[77]->GetAsAtkComponentNode()->Component->UldManager.NodeList[0]->GetAsAtkComponentNode()->Component->UldManager.NodeList[2]->GetAsAtkTextNode()->NodeText.ToString();
            return DawnInfo;
        }

        private static AtkUnitBase* ToDolistAddon => (AtkUnitBase*)Dalamud.GameGui.GetAddonByName("_ToDoList", 1);
        public static Dictionary<uint, string> GetToDolistInfo()
        {
            Dictionary<uint, string> ret =new();
            for(var i = 0;i < ToDolistAddon->UldManager.NodeListCount; i++)
            {
                var NodeID = ToDolistAddon->UldManager.NodeList[i]->NodeID;
                if (NodeID > 21000 && NodeID < 21100)
                {
                    ret.Add(NodeID,ToDolistAddon->UldManager.NodeList[i]->GetAsAtkComponentNode()->Component->UldManager.SearchNodeById(6)->GetAsAtkTextNode()->NodeText.ToString());
                }
            }
            return ret;
        }
        public static void GetDutyTimeRemaining(out uint min,out uint second)
        {
            var text = ToDolistAddon->UldManager.NodeList[9]->GetAsAtkComponentNode()->Component->UldManager.NodeList[8]->GetAsAtkTextNode()->NodeText.ToString();
            var index = text.IndexOf(":");
            min = 0; second = 0;
            if (index == -1)
                return;
            min = uint.Parse(text.Substring(0,index));
            second = uint.Parse((text.Substring(index + 1)));
        }
        public static uint DutyStep()
        {
            var ToDolistInfo = GetToDolistInfo();
            var now = 0;
            foreach(var item in ToDolistInfo)
            {
                if(item.Value != "???")
                    now++;
            }
            return (uint)(now - 1);
        }
        #endregion
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
        #region text
        private static IntPtr Trun
        {
            get
            {
                if(Dalamud.SigScanner.TryScanText("40 53 48 83 EC 20 F3 0F 10 81 B0 00 00 00 48 8B D9 0F 2E C1", out var ptr))
                    return ptr;
                return IntPtr.Zero;
            }
        }
        public static void TrunR(float ration)
        {
            if (Trun == IntPtr.Zero)
                return;
            if (Dalamud.ClientState.LocalPlayer == null)
                return;
            ((delegate*<long, float, long>)Trun)((long)Dalamud.ClientState.LocalPlayer.Address, ration);
        }
        #endregion
    }
}
