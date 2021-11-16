using Godot;
using System;
using MapSystem;

public class TestScene : Node2D
{
    Random RND = new Random();
    Sector Map = new Sector(100);

    FileDialog Dialog;

    CellMapFile GlobMap = new CellMapFile(100);

    TileSet MapTileSet = ResourceLoader.Load<TileSet>("res://TileSets/TestTileset.tres");

    MapCell[,] CreateTestMapCell(int Size){
        MapCell[,] Result = new MapCell[Size,Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Result[i,j].TileID = 34;
            }
        }
        return Result;
    }
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        System.IO.Directory.CreateDirectory(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below");
        GlobMap.OpenFile(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/Map.bin");
        Map.Viewport.TileSet = MapTileSet;
        Map.Viewport.CellSize = new Vector2(10,10);
        this.AddChild(Map.Viewport);
    }

    public override void _Process(float delta)
    {
        GlobMap.WriteSector(CreateTestMapCell(GlobMap.SectorSize));
        GD.Print(GlobMap.ReadSector()[0,0].TileID);
        GD.Print(GlobMap.ReadSector()[99,99].TileID);
        GD.Print(GlobMap.ReadSector().GetLength(0));
        GD.Print(GlobMap.ReadSector().GetLength(1));
    }

    public override void _ExitTree()
    {
        GlobMap.CloseFile();
        base._ExitTree();
    }
}
