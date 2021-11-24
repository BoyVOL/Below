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

        public Sector(MapCell[,] cells){
            Cells = cells;
        }

        public string StringContent(){
            string Result = "";
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Result += Cells[i,j].TileID+" ";
                }
                Result += "\n";
            }
            return Result;
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
        public int[] Center = new int[] {5,5};
        public Sector[,] Buffer = new Sector[10,10];

        /// <summary>
        /// Загружает буффер секторами вокруг общего центра
        /// </summary>
        public void LoadBuffer(){
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                for (int j = 0; j < Buffer.GetLength(1); j++)
                {
                    Buffer[i,j] = GetSector(new int[] {
                        Center[0]-Buffer.GetLength(0)/2+i,
                        Center[1]-Buffer.GetLength(1)/2+j
                    });
                }
            }
        }
    }

    /// <summary>
    /// Дочерний класс словаря, отвечающий за представление секторов в файле
    /// </summary>
    public class SectorDict : Dictionary<double, SuperSectorData>{

        public SectorRecordsFile Records;

        /// <summary>
        /// Метод для загрузки словаря, содержащего соотношение индексов суперсекторов с их координатами в виде double
        /// </summary>
        public void LoadDictionary(){
            SuperSectorRecord[] Temp = Records.ReadRecords();
            this.Clear();
            for (int i = 0; i < Temp.Length; i++)
            {
                this.Add(KeyFromPos(Temp[i].x,Temp[i].y),Temp[i].Data);
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
            SuperSectorData[] Values = new SuperSectorData[this.Count];
            this.Keys.CopyTo(Keys,0);
            this.Values.CopyTo(Values,0);
            SuperSectorRecord[] Temp = new SuperSectorRecord[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                int[] TempPos = PosFromKey(Keys[i]);
                Temp[i].x = TempPos[0];
                Temp[i].y = TempPos[1];
                Temp[i].Data = Values[i];
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
        /// Метод для получения сектора по указанным координатам относительно общей сетки секторов, из общей системы файлов
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public Sector GetSector(int[] Pos){
            int[] SSC = SupSecCoords(Pos);
            int[] ISC = InSectorCoords(Pos);
            SuperSectorData Buff;
            //Проверка на наличие суперсектора в файле
            if(SuperSecDict.TryGetValue(SuperSecDict.KeyFromPos(SSC[0],SSC[1]),out Buff)){
                long ID = Buff.filePos;
                ID+=Data.GetIDShift(ISC[0],ISC[1]);
                return new Sector(Data.ReadSector(ID));
            }
            else throw new Exception("There's no supersector with coords "+SSC[0]+" "+SSC[1]);
        }

        /// <summary>
        /// Метод сохранения сектора по указанным координатам
        /// </summary>
        /// <param name="Pos"></param>
        public void SaveSector(Sector sector, int[] Pos){
            int[] SSC = SupSecCoords(Pos);
            int[] ISC = InSectorCoords(Pos);
            SuperSectorData Buff;
            //Проверка на наличие суперсектора в файле
            if(SuperSecDict.TryGetValue(SuperSecDict.KeyFromPos(SSC[0],SSC[1]),out Buff)){
                long ID = Buff.filePos;
                ID+=Data.GetIDShift(ISC[0],ISC[1]);
                Data.WriteSector(sector.Cells, ID);
            }
            else throw new Exception("There's no supersector with coords "+SSC[0]+" "+SSC[1]);
        }

        /// <summary>
        /// Метод получения координат суперсектора на основе координат сектора
        /// </summary>
        /// <param name="SectGlobCoords">положение сектора в глобальной сетке секторов, суперсектор которого надо определить</param>
        /// <returns></returns>
        public int[] SupSecCoords(int[] SectGlobCoords){
            return new int[] {
                (int)Math.Truncate((decimal)SectGlobCoords[0]/Data.SupersectorSize),
                (int)Math.Truncate((decimal)SectGlobCoords[1]/Data.SupersectorSize)
                };
        }

        /// <summary>
        /// Метод преобразовывающий глобальные координаты сектора в сетке секторов в координаты сектора внутри сегмента
        /// </summary>
        /// <param name="SectGlobCoords"></param>
        /// <returns></returns>
        public int[] InSectorCoords(int[] SectGlobCoords){
            return new int[] {
                (int)SectGlobCoords[0]%Data.SupersectorSize,
                (int)SectGlobCoords[1]%Data.SupersectorSize
                };
        }
    }
}