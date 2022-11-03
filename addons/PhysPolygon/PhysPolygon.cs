using Godot;
using System;

[Tool]
public class PhysPolygon : EditorPlugin
{
    public string LibDir = "res://addons/PhysPolygon/";

    public void AddCustomType(string Type, string parent){
        AddCustomType(Type,parent,GD.Load<Script>(LibDir+Type+"/"+Type+".cs"),
        GD.Load<Texture>(LibDir+"/"+Type+"/icon.png"));
    }

    public void AddAutoloadSingleton(string Name){
        AddAutoloadSingleton(Name,LibDir+"/AutoloadScenes/"+Name+".tscn");
    }
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        
    }

    public override void _ExitTree(){
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
