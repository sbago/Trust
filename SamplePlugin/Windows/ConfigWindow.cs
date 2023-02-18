using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ImGuiScene;

namespace Trust.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    private long ptr;
    private int index;
    private IntPtr GetPos;
    private IntPtr SetPos;
    private IntPtr MouseToWorld;
    public ConfigWindow(Plugin plugin) : base(
        "Trust",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        //this.Size = new Vector2(232, 75);
        //this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
        //E8 ?? ?? ?? ?? 66 83 8B ?? ?? ?? ?? ?? 33 D2
        GetPos = Dalamud.SigScanner.ScanText("83 79 7C 00 75 09 F6 81 ?? ?? ?? ?? ?? 74 2A");
        SetPos = Dalamud.SigScanner.ScanText("E8 ?? ?? ?? ?? 66 83 8B ?? ?? ?? ?? ?? 33 D2");
        MouseToWorld = Dalamud.SigScanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B E9 48 8B DA 48 8D 0D");
    }

    public void Dispose() { }

    private string version;
    private int duty;
    private Character character;
    public unsafe override void Draw()
    {
        // can't ref a property, so use a local copy
        var configValue = this.Configuration.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Config Bool", ref configValue))
        {
            this.Configuration.SomePropertyToBeSavedAndWithADefault = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }
        if(ImGui.Button("亲信界面"))
        {
            Func.OpenDawn();
        }
        if (ImGui.BeginCombo("版本", version))
        {
            foreach(var v in Enum.GetNames(typeof(Version)))
            if (ImGui.Selectable(v))
            {
                version = v;
                Enum.Parse(typeof(Version), v);
                Func.SelectVersion((Version)Enum.Parse(typeof(Version), v));
            }
            ImGui.EndCombo();
        }
        if (ImGui.BeginCombo("副本", duty.ToString()))
        {
            for(int i = 0; i < 10;i++)
            {
                if (ImGui.Selectable(i.ToString()))
                {
                    duty = i;
                    Func.SelectDuty((uint)i);
                }
            }
            ImGui.EndCombo();
        }

        if (ImGui.BeginCombo("选人", character.ToString()))
        {
            foreach (var v in Enum.GetNames(typeof(Character)))
                if (ImGui.Selectable(v))
                {
                    character = (Character)Enum.Parse(typeof(Character), v);
                    Func.SelectCharacter(character);
                }
            ImGui.EndCombo();
        }
        if (ImGui.Button("进入"))
        {
            Func.Fire();
        }
        ImGui.SameLine();
        if(ImGui.Button("出发"))
        {
            Func.SelectYesContentsFinderConfirm();
        }
        ImGui.SameLine();
        if (ImGui.Button("离开副本"))
        {
            Func.LeaveDuty();
        }
        ImGui.Text(ptr.ToString("x"));
        ImGui.InputInt("#1", ref index);
        if(ImGui.Button("更新"))
        {
            ptr = (long)((AtkUnitBase*)Dalamud.GameGui.GetAddonByName("Dawn", 1))->AtkEventListener.vfunc[index];
            ImGui.SetClipboardText($"{ptr:x}");
        }
        ImGui.SameLine();
        if (ImGui.Button("auto"))
        {
            Func.AutoSelectYesContentsFinderConfirm();
        }
        ImGui.SameLine();
        if (ImGui.Button("l"))
        {
            var a = Func.DutyStep();
            PluginLog.Information(a.ToString());
            //Func.RemovAutoSelectYesContentsFinderConfirmEvent();
        }
    }
}
