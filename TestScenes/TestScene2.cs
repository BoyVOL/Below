using Godot;
using System;
using MapSystem;

public class TestScene2 : Node2D
{
    public void CheckCoords(int[] input){
        GD.Print(Loader.SupSecCoords(input)[0],",",Loader.SupSecCoords(input)[1]);
    }
    SectorLoader Loader = new SectorLoader();

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        System.IO.Directory.CreateDirectory(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below");
        SectorRecordsFile Sectors = new SectorRecordsFile();
        SupersectorFile SupSectors = new SupersectorFile(10,10);
        Sectors.FilePath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/MapAlloc.bin";
        Sectors.OpenFile();
        SupSectors.FilePath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/Map.bin";
        SupSectors.OpenFile();
        Loader.Data = SupSectors;
        Loader.SuperSecDict.Records = Sectors;
        CheckCoords(new int[]{1,1});
        CheckCoords(new int[]{9,9});
        CheckCoords(new int[]{10,9});
    }

    public override void _Process(float delta)
    {
        Loader.SuperSecDict.LoadDictionary();
        Loader.GetSector(new int[]{0,0});
        Loader.GetSector(new int[]{1,0});
        Loader.SuperSecDict.SaveDictionary();
    }
}
