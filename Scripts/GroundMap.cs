using Godot;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GroundMap{

    [Serializable()]
    /// <summary>
    /// Структура данных об ячейке
    /// </summary>
    public struct MapCell{

        /// <summary>
        /// Идентификатор, какой айди должен быть для отрисовки данной ячейки
        /// </summary>
        public byte TileID;
        
        public MapCell(byte[] data){
            TileID = data[0];
        }

        public byte[] ToByte(){
            byte[] Result = new byte[1];
            Result[0] = TileID;
            return Result;
        }

        public int ByteLength(){
            return ToByte().Length;
        }
    }

    /// <summary>
    /// Структура, отвечающая за расположение сектора на глобальной карте
    /// </summary>
    public struct SectorCoords{

        public int x;

        public int y;

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

    /// <summary>
    /// Класс для создания и сохранения глобальной карты
    /// </summary>
    public class CellMapFile{

        FileStream File = null;

        public readonly int SectorSize;

        public readonly int SectorByteSize;

        public readonly int CellByteLength;

        public CellMapFile(int sectorSize){
            SectorSize = sectorSize;
            MapCell[,] Temp = new MapCell[sectorSize,sectorSize];
            CellByteLength = new MapCell().ByteLength();
            SectorByteSize = Temp.Length*CellByteLength;
        }

        public void OpenFile(string Filepath){
            File = new FileStream(Filepath,FileMode.OpenOrCreate, 
            FileAccess.ReadWrite, FileShare.None);
        }

        public void ReloadFile(string Filepath){
            File = new FileStream(Filepath,FileMode.CreateNew, 
            FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Метод для чтения секторов из файла
        /// </summary>
        /// <returns></returns>
        public MapCell[,] ReadSector(){
            byte[] Temp = new byte[CellByteLength];
            MapCell[,] Result = new MapCell[SectorSize,SectorSize];
            int Pointer = 0;
            File.Seek(Pointer,SeekOrigin.Begin);
            for (int i = 0; i < SectorSize; i++)
            {
                for (int j = 0; j < SectorSize; j++)
                {
                    Pointer += File.Read(Temp,0,CellByteLength);
                    Result[i,j] = new MapCell(Temp);
                }
            }
            return Result;
        }

        /// <summary>
        /// Метод для записи секторов в файл
        /// </summary>
        /// <param name="sector"></param>
        public void WriteSector(MapCell[,] sector){
            byte[] Temp = new byte[CellByteLength];
            int Pointer = 0;
            File.Seek(Pointer,SeekOrigin.Begin);
            for (int i = 0; i < SectorSize; i++)
            {
                for (int j = 0; j < SectorSize; j++)
                {
                    Temp = sector[i,j].ToByte();
                    File.Write(Temp,0,CellByteLength);
                    Pointer+=CellByteLength;
                }
            }
        }
        
        public void CloseFile(){
            File.Close();
        }
    }
    
    /// <summary>
    /// Класс для генерации глобальной карты
    /// </summary>
    public class MapGenerator{
        
    }
}
