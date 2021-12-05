using Godot;
using System;
using MapSystem;

public class CollisionTest : Node2D
{
    MapGenerator Gen = new MapGenerator();

    Sector Sect = new Sector(10);
    
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
