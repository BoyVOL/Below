using Godot;
using System;
using MapSystem;

public class TestScene : Node2D
{
    Random RND = new Random();
    Sector Map = new Sector(100,1);

    FileDialog Dialog;

    CellMapFile GlobMap = new CellMapFile(100);

    SectorRecordsFile Sectors = new SectorRecordsFile();

    SupersectorFile SupSectors = new SupersectorFile(10,10);

    TileSet MapTileSet = ResourceLoader.Load<TileSet>("res://TileSets/TestTileset.tres");

    MapCell[,] CreateTestMapCell(int Size){
        Random Rnd = new Random();
        MapCell[,] Result = new MapCell[Size,Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Result[i,j].TileID = (byte)Rnd.Next(0,30);
            }
        }
        return Result;
    }

    MapCell[,][,] CreateTestSupSector(int size1, int size2){
        Random Rnd = new Random();
         MapCell[,][,] Result = new MapCell[size1,size1][,];
        for (int i = 0; i < size1; i++)
        {
            for (int j = 0; j < size1; j++)
            {
                Result[i,j] = CreateTestMapCell(size2);
            }
        }
        return Result;
    }

    /// <summary>
    /// Создаёт таблицу секторов указанного размера на основе случайных величин
    /// </summary>
    /// <param name="Size"></param>
    /// <returns></returns>
    SuperSectorRecord[] CreateRandomSectorTable(int Size){
        Random Rnd = new Random();
        SuperSectorRecord[] Result = new SuperSectorRecord[Size];
        for (int i = 0; i < Size; i++)
        {
            Result[i].x = (int)Rnd.Next(0,100000);
            Result[i].y = (int)Rnd.Next(0,100000);
            Result[i].Data.filePos = (int)Rnd.Next(0,100000);
        }
        return Result;
    }

    /// <summary>
    /// Создаёт таблицу квадрата секторов с указанной стороной
    /// </summary>
    /// <param name="Size"></param>
    /// <returns></returns>
    SuperSectorRecord[] CreateSectorTable(int Size){
        Random Rnd = new Random();
        SuperSectorRecord[] Result = new SuperSectorRecord[Size*Size];
        int ID = 0;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Result[ID].x = i;
                Result[ID].y = j;
                Result[ID].Data.filePos = ID;
                ID++;
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
        //GlobMap.OpenFile(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/Map.bin");
        Sectors.FilePath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/MapAlloc.bin";
        Sectors.OpenFile();
        SupSectors.FilePath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/Map.bin";
        SupSectors.OpenFile();
        Map.Map.TileSet = MapTileSet;
        Map.Map.CellSize = new Vector2(10,10);
        this.AddChild(Map.Map);
    }

    void TestSectorRW(){
        GlobMap.WriteSector(CreateTestMapCell(GlobMap.SectorSize),0);
        GlobMap.WriteSector(CreateTestMapCell(GlobMap.SectorSize),GlobMap.SectorByteSize);
        GD.Print(GlobMap.ReadSector(GlobMap.SectorByteSize)[0,0].TileID);
        GD.Print(GlobMap.ReadSector(GlobMap.SectorByteSize)[99,99].TileID);
        GD.Print(GlobMap.ReadSector(GlobMap.SectorByteSize).GetLength(0));
        GD.Print(GlobMap.ReadSector(GlobMap.SectorByteSize).GetLength(1));
    }

    void TestRecordsRW(){
        SuperSectorRecord[] Temp = CreateSectorTable(10);
        GD.Print("before = ",Temp[99].x);
        GD.Print("before = ",Temp[99].y);
        GD.Print("before = ",Temp[99].Data.filePos);
        Sectors.WriteRecords(Temp);
        Temp = Sectors.ReadRecords();
        GD.Print("After = ",Temp[99].x);
        GD.Print("After = ",Temp[99].y);
        GD.Print("After = ",Temp[99].Data.filePos);
    }

    void TestSupSectors(){
        MapCell[,][,] Temp = CreateTestSupSector(10,10);
        GD.Print("SupSec before = ",Temp[0,0][0,0].TileID);
        GD.Print("SupSec before = ",Temp[9,9][9,9].TileID);
        SupSectors.WriteSupersector(Temp,0);
        Temp = SupSectors.ReadSupersector(0);
        GD.Print("SupSec after = ",Temp[0,0][0,0].TileID);
        GD.Print("SupSec after = ",Temp[9,9][9,9].TileID);
    }

    public override void _Process(float delta)
    {
        TestSupSectors();
        TestRecordsRW();
    }

    public override void _ExitTree()
    {
        GlobMap.CloseFile();
        base._ExitTree();
    }
}
