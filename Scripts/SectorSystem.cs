using Godot;
using System.Collections.Generic;
using System;

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
    /// Дочерний класс словаря, отвечающий за представление секторов в файле
    /// </summary>
    public class SectorDict : Dictionary<double, int>{

        public SectorRecordsFile Records;

        /// <summary>
        /// Метод для загрузки словаря, содержащего соотношение индексов суперсекторов с их координатами в виде double
        /// </summary>
        public void LoadDictionary(){
            SectorRecord[] Temp = Records.ReadRecords();
            this.Clear();
            for (int i = 0; i < Temp.Length; i++)
            {
                this.Add(KeyFromPos(Temp[i].x,Temp[i].y),Temp[i].filePos);
            }
        }

        /// <summary>
        /// Возвращает ключ double из набора координат
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double KeyFromPos(int x, int y){
            return (double)x*(double)Int32.MaxValue+y;
        }

        /// <summary>
        /// Возвращает массив координат, полученных из ключа double
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public int[] PosFromKey(double Key){
            int [] Result = new int[2];
            Result[0] = (int)((double)Key/(double)Int32.MaxValue);
            Result[1] = (int)((double)Key%(double)Int32.MaxValue);
            return Result;
        }

        /// <summary>
        /// метод для сохранения всех записей в словаре в виде файла
        /// </summary>
        public void SaveDictionary(){
            double[] Keys = new double[this.Count];
            int[] Values = new int[this.Count];
            this.Keys.CopyTo(Keys,0);
            this.Values.CopyTo(Values,0);
            SectorRecord[] Temp = new SectorRecord[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                int[] TempPos = PosFromKey(Keys[i]);
                Temp[i].x = TempPos[0];
                Temp[i].y = TempPos[1];
                Temp[i].filePos = Values[i];
            }
            Records.WriteRecords(Temp);
        }
    }

    /// <summary>
    /// Класс для управления загрузкой секторов и их содержимого
    /// </summary>
    public class SectorLoader{
        public SupersectorFile Data;
        
        public SectorDict SuperSecDict = new SectorDict();

        /// <summary>
        /// Метод для получения сектора, левый верхний край которого содержит указанные координаты, из общей системы файлов
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public Sector GetSector(int[] Pos){
            Sector Result = new Sector(Data.SectorSize);
            GD.Print(SuperSecDict[SuperSecDict.KeyFromPos(Pos[0],Pos[1])]);
            return Result;
        }

        /// <summary>
        /// Метод получения координат суперсектора на основе координат сектора
        /// </summary>
        /// <param name="Pos">положение сектора, суперсектор которой надо определить</param>
        /// <returns></returns>
        public int[] SupSecCoords(int[] Pos){
            return new int[] {
                (int)Math.Truncate((decimal)Pos[0]/Data.SupersectorSize),
                (int)Math.Truncate((decimal)Pos[1]/Data.SupersectorSize)
                };
        }
    }
}