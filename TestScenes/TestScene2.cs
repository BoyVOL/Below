using Godot;
using System;
using MapSystem;

public class TestScene2 : Node2D
{
    public void CheckCoords(int[] input){
        GD.Print(Loader.SupSecCoords(input)[0],",",Loader.SupSecCoords(input)[1]);
        GD.Print(Loader.InSectorCoords(input)[0],",",Loader.InSectorCoords(input)[1]);
    }
    SectorBuffer Loader = new SectorBuffer(1,3);

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
        CheckCoords(new int[]{10,1});
        Loader.SuperSecDict.LoadDictionary();


        GD.Print(Loader.GetSector(new int[]{0,0}).StringContent());
        Loader.BufferStart = new int[] {0,0};
        Loader.LoadBuffer();
        Loader.Buffer[0,0].Cells[0,0].TileID=50;
        GD.Print(Loader.Buffer[0,0].StringContent());
        Loader.MoveRight();
        Loader.MoveUp();
        GD.Print(Loader.GetSector(new int[]{1,1}).StringContent());
        GD.Print(Loader.Buffer[0,0].StringContent());
        GD.Print(Loader.GetSector(new int[]{2,2}).StringContent());
        GD.Print(Loader.Buffer[1,1].StringContent());
        GD.Print(Loader.GetSector(new int[]{3,3}).StringContent());
        GD.Print(Loader.Buffer[2,2].StringContent());
        Loader.SaveBuffer();
    }

    public override void _Process(float delta)
    {
    }

    public override void _ExitTree()
    {
        Loader.SuperSecDict.SaveDictionary();
        base._ExitTree();
    }
}
