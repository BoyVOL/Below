using Godot;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GroundMap{

    public struct MapCell{
        public int TileID;
    }

    /// <summary>
    /// Класс, отвечающий за загрузку части карты в игре
    /// </summary>
    public class Sector{
        public TileMap Viewport = new TileMap();

        public MapCell[,] Cells;

        public Sector(int Size){
            Cells = new MapCell[Size,Size];
        }

        public void Visualise(){
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Viewport.SetCell(i,j,Cells[i,j].TileID);
                }
            }
        }
    }

    public class MapSerialiser{
        
        BinaryFormatter Formatter = new BinaryFormatter();

        MemoryStream stream;
        public Byte[] Serialise(MapCell[,] cells){
            stream = new MemoryStream();
            Formatter.Serialize(stream,cells);
            return stream.ToArray();
        }

        public MapCell[,] Deserialise(Byte[] Data){
            stream = new MemoryStream(Data);
            return (MapCell[,])Formatter.Deserialize(stream);
        }
    }

    /// <summary>
    /// Класс для создания и сохранения глобальной карты
    /// </summary>
    public class GlobalMap{

        /// <summary>
        /// Поле, содержащее, куда надо подгружать карту
        /// </summary>
        public Node2D Parent = null;

        /// <summary>
        /// Поле, содержащее глобальный тайлсет для карты
        /// </summary>
        public TileSet Set = null;
    }
    
    /// <summary>
    /// Класс для генерации глобальной карты
    /// </summary>
    public class MapGenerator{
        
    }
}
