using Godot;
using System;
using GroundMap;

public class TestScene : Node2D
{
    Random RND = new Random();
    Sector Map = new Sector(100);

    TileSet MapTileSet = ResourceLoader.Load<TileSet>("res://TileSets/TestTileset.tres");
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Map.Viewport.TileSet = MapTileSet;
        Map.Viewport.CellSize = new Vector2(10,10);
        this.AddChild(Map.Viewport);
    }

    public override void _Process(float delta)
    {
        Map.Visualise();
    }
}
