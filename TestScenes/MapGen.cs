using Godot;
using System;
using MapSystem;

public class MapGen : Node2D
{
    MapGenerator Gen = new MapGenerator();
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public void GenerateSupersectors(int Size){
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Gen.GenerateSupersector(i,j);
            }
        }
    }

    public override void _Ready()
    {
        System.IO.Directory.CreateDirectory(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below");
        SectorRecordsFile Sectors = new SectorRecordsFile();
        SupersectorFile SupSectors = new SupersectorFile(10,10);
        Sectors.FilePath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/MapAlloc.bin";
        Sectors.OpenFile();
        Sectors.ReloadFile();
        SupSectors.FilePath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/Map.bin";
        SupSectors.OpenFile();
        SupSectors.ReloadFile();
        Gen.Data = SupSectors;
        Gen.Dict.Records = Sectors;
        GenerateSupersectors(2);
        Gen.Dict.SaveDictionary();
        Gen.Data.CloseFile();
        Gen.Dict.Records.CloseFile();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
