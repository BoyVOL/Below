using Godot;
using System;

namespace GroundMap{

    public struct MapPoint{
        int TileID;
    }

    /// <summary>
    /// Класс, отвечающий за загрузку карты в физическом варианте
    /// </summary>
    public class localMap{
        
    }

    /// <summary>
    /// Класс для создания глобальной карты
    /// </summary>
    public class GlobalMap{

    }
    
    /// <summary>
    /// Класс для генерации глобальной карты
    /// </summary>
    public class MapGenerator{
        
    }

    /// <summary>
    /// Класс, отвечающий за отображение карты на экране
    /// </summary>
    public class MapVisual{
        public localMap Map;

        public TileMap Current = new TileMap();

        
    }
}
