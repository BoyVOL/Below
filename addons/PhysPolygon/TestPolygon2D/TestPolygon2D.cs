using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public class TestPolygon2D : Polygon2D{

    [Export]
    float maxLength = 100;

    [Export]
    float minLength = 1;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
            List<Vector2> poly = new List<Vector2>(Polygon);
            GD.Print(poly.ToArray().Length);
            Polygon = poly.ToArray();

        int i = 0;
        while (i < poly.Count-1){
            float dist = poly[i].DistanceSquaredTo(poly[i+1]);
            GD.Print(dist);
            i++;
        }
    }
}