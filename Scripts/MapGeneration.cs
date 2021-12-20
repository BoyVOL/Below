using Godot;
using System;

namespace MapSystem{
    public class MapGenerator{
        public SupersectorFile Data;
        public SectorDict Dict = new SectorDict();

        /// <summary>
        /// Метод для создания базового наполнения сектора, как будто там ничего нет изначально
        /// </summary>
        /// <returns></returns>
        public MapCell[,] BaseSectorData(){
            Random Rnd = new Random();
            MapCell[,] Result = new MapCell[Data.SectorSize,Data.SectorSize];
            for (int i = 0; i < Data.SectorSize; i++)
            {
                for (int j = 0; j < Data.SectorSize; j++)
                {
                    Result[i,j].TileID = (byte)Rnd.Next(0,100);
                }
            }
            return Result;
        }

        /// <summary>
        /// Метод для создания и записи в соответствующий файл суперсектора
        /// </summary>
        /// <param name="x">координата x сектора</param>
        /// <param name="y">координата y сектора</param>
        public MapCell[,][,] SupersectorBase(){
            MapCell[,][,] Result = new MapCell[Data.SupersectorSize,Data.SupersectorSize][,];
            for (int i = 0; i < Data.SupersectorSize; i++)
            {
                for (int j = 0; j < Data.SupersectorSize; j++)
                {
                    Result[i,j] = BaseSectorData();
                }
            }
            GD.Print(new Sector(Result[0,0],1).StringContent());
            return Result;
        }

        /// <summary>
        /// Метод записи сектора в файл
        /// </summary>
        /// <param name="Sector"></param>
        public void GenerateSupersector(int x, int y){
            long LastID = Data.GetLength();
            GD.Print(LastID);
            GD.Print(Dict.KeyFromPos(x,y));
            Data.WriteSupersector(SupersectorBase(),LastID);
            SuperSectorData sectorData = new SuperSectorData();
            sectorData.filePos = LastID;
            Dict.Add(Dict.KeyFromPos(x,y),sectorData);
        }
    }
}