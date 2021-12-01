using Godot;

namespace MapSystem
{
    /// <summary>
    /// Абстрактный класс для определения сетки коллизий
    /// </summary>
    public abstract class CollGrid{
        Transform2D Transform;

        Transform2D Shift;

        Vector2 Size;
    }
}