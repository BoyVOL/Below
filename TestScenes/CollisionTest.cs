using Godot;
using System;
using MapSystem;

public class CollisionTest : Node2D
{

    WorldController Controller = new WorldController(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/Map.bin",
    System.Environment.GetEnvironmentVariable("USERPROFILE")+"/AppData/Local/Below/MapAlloc.bin",1,10);

    TestGrid Grid = new TestGrid();
    
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

    void GridSetup(){
        Grid.size = new int[] {10,10};
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Controller.Init(1,1);
        Controller.Buffer.ShiftTo(new Vector2(10,10));
        Controller.Buffer.ShiftTo(new Vector2(20,10));
        Controller.Buffer.ShiftTo(new Vector2(-10,10));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
