using Godot;

public class TestPolygon2D : Polygon2D{

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        foreach (Vector2 point in Polygon){
            GD.Print(point);
        }
    }
}