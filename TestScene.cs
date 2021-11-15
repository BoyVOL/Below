using Godot;
using System;
using GroundMap;

public class TestScene : Node2D
{
    Random RND = new Random();
    MapVisual Map = new MapVisual();

    TileSet MapTileSet = ResourceLoader.Load<TileSet>("res://TileSets/TestTileset.tres");
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Map.Current.TileSet = MapTileSet;
        Map.Current.CellSize = new Vector2(10,10);
        this.AddChild(Map.Current);
    }

    public override void _Process(float delta)
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                Map.Current.SetCellv(new Vector2(i,j),RND.Next(-1,4));
            }
        }
    }
}
