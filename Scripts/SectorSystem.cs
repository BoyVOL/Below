using Godot;
using System.Collections.Generic;

namespace MapSystem{
    /// <summary>
    /// Класс, отвечающий за загрузку части карты в игре
    /// </summary>
    public class Sector{
        public TileMap Map = new TileMap();

        public MapCell[,] Cells;

        Vector2 Coords;

        public Sector(int Size){
            Cells = new MapCell[Size,Size];
        }

        public void Visualise(){
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Map.SetCell(i,j,Cells[i,j].TileID);
                }
            }
        }
    }

    /// <summary>
    /// Класс для подгрузки и управления загруженными секторами
    /// </summary>
    public class SectorBuffer : SectorLoader{
        public Vector2 Center;
        public Sector[,] Buffer;

                
    }

    /// <summary>
    /// Класс для управления загрузкой секторов и их содержимого
    /// </summary>
    public class SectorLoader{
        public SupersectorFile Data;

        public SectorRecordsFile Records;
        
        Dictionary<int[],int> SupersectorRecords = new Dictionary<int[], int>();

        /// <summary>
        /// Метод для получения сектора, включающего указанные координаты, из общей системы файлов
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public Sector GetSector(Vector2 Pos){
            Sector Result = new Sector(Data.SectorSize);
            return Result;
        }

        /// <summary>
        /// Метод для загрузки словаря, содержащего соотношение индексов суперсекторов с их координатами
        /// </summary>
        public void LoadDictionary(){
            SectorRecord[] Temp = Records.ReadRecords();
            for (int i = 0; i < Temp.Length; i++)
            {
                SupersectorRecords.Add(new int[] {Temp[i].x,Temp[i].y},Temp[i].filePos);
            }
        }

        public void SaveDictionary(){
            int[][] Keys = new int[SupersectorRecords.Count][];
            int[] Values = new int[SupersectorRecords.Count];
            SupersectorRecords.Keys.CopyTo(Keys,0);
            SupersectorRecords.Values.CopyTo(Values,0);
            SectorRecord[] Temp = new SectorRecord[SupersectorRecords.Count];
            Records.ReloadFile();
            for (int i = 0; i < SupersectorRecords.Count; i++)
            {
                Temp[i].x = Keys[i][0];
                Temp[i].y = Keys[i][1];
                Temp[i].filePos = Values[i];
            }
            Records.WriteRecords(Temp);
        }

        public void SaveSector(Sector sector){

        }
    }
}