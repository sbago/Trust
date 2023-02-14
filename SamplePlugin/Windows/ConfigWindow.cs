using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Trust.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Trust",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        //this.Size = new Vector2(232, 75);
        //this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    private string version;
    private int duty;
    private Character character;
    public override void Draw()
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
        if (ImGui.Button("开始"))
        {
            Func.Fire();
        }
        ImGui.SameLine();
        if(ImGui.Button("出发"))
        {
            Func.SelectYesContentsFinderConfirm();
        }
    }
}
