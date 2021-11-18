using Godot;
using System;
using System.IO;

namespace MapSystem{
    /// <summary>
    /// Структура данных об ячейке
    /// </summary>
    public struct MapCell{

        /// <summary>
        /// Идентификатор, какой айди должен быть для отрисовки данной ячейки
        /// </summary>
        public byte TileID;

        public const int ByteLength = 1;
        
        public MapCell(byte[] data){
            TileID = data[0];
        }

        public byte[] ToByte(){
            byte[] Result = new byte[ByteLength];
            Result[0] = TileID;
            return Result;
        }
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


    public class Supersector{
        
    }
    
    /// <summary>
    /// Класс для генерации глобальной карты
    /// </summary>
    public class MapGenerator{
        
    }
}