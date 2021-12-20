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

        public readonly float CellSize;

        public Sector(int Size,float cellSize){
            Cells = new MapCell[Size,Size];
            CellSize = cellSize;
        }

        public Sector(MapCell[,] cells,float cellSize){
            Cells = cells;
            CellSize = cellSize;
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

        /// <summary>
        /// Выделяет из координат относительно начала сектора ту часть, которая отвечает за расположение точки внутри ячейки
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public Vector2 GetCoordsInsideCell(Vector2 Pos){
            return new Vector2(
                Pos.x%CellSize,
                Pos.y%CellSize
            );
        }

        /// <summary>
        /// Метод, позволяющий определить индекс ячейки по координатам относительно начала сектора
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public int[] GetCellID(Vector2 Pos){
            return new int[] {
                (int)Math.Truncate((decimal)(Pos.x/CellSize)),
                (int)Math.Truncate((decimal)(Pos.y/CellSize))
                };
        }
    }

    /// <summary>
    /// Класс для подгрузки и управления загруженными секторами
    /// </summary>
    public class SectorBuffer : SectorLoader{
        
        /// <summary>
        /// Начальная точка буффера - верхняя левая точка
        /// </summary>
        /// <value></value>
        public int[] BufferStart = new int[] {0,0};
        public Sector[,] Buffer = new Sector[3,3];

        public SectorBuffer(float cellSize) : base(cellSize){

        }

        /// <summary>
        /// Загружает буффер секторами вокруг общего центра
        /// </summary>
        public void LoadBuffer(){
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                for (int j = 0; j < Buffer.GetLength(1); j++)
                {
                    Buffer[i,j] = GetSector(new int[] {
                        BufferStart[0]+i,
                        BufferStart[1]+j
                    });
                }
            }
        }

        /// <summary>
        /// Метод для сохранения всего буффера
        /// </summary>        
        public void SaveBuffer(){
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                for (int j = 0; j < Buffer.GetLength(1); j++)
                {
                    SaveSector(Buffer[i,j],new int[] {
                        BufferStart[0]+i,
                        BufferStart[1]+j
                    });
                }
            }
        }

        /// <summary>
        /// Загрузка колонки по указанному индексу
        /// </summary>
        /// <param name="colID"></param>
        void LoadColumn(int colID){
            for (int i = 0; i < Buffer.GetLength(1); i++)
            {
                Buffer[colID,i] = GetSector(new int[] {
                    BufferStart[0]+colID,
                    BufferStart[1]+i
                });
            }
        }

        void LoadRow(int rowID){
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                Buffer[i,rowID] = GetSector(new int[] {
                    BufferStart[0]+i,
                    BufferStart[1]+rowID
                });
            }
        }
        
        void SaveColumn(int colID){
            for (int i = 0; i < Buffer.GetLength(1); i++)
            {
                SaveSector(Buffer[colID,i],new int[] {
                    BufferStart[0]+colID,
                    BufferStart[1]+i
                });
            }
        }

        void SaveRow(int rowID){
            for (int i = 0; i < Buffer.GetLength(1); i++)
            {
                SaveSector(Buffer[i,rowID],new int[] {
                    BufferStart[0]+i,
                    BufferStart[1]+rowID
                });
            }
        }

        /// <summary>
        /// смещение указанной колонки на указанное положение
        /// </summary>
        /// <param name="rowID"></param>
        /// <param name="shift"></param>

        void ShiftColumn(int colID, int shift){
            for (int i = 0; i < Buffer.GetLength(1); i++)
            {
                Buffer[colID+shift,i] = Buffer[colID,i];
            }
        }

        /// <summary>
        /// смещение указанной строки на указанное положение
        /// </summary>
        /// <param name="rowID"></param>
        /// <param name="shift"></param>
        void ShiftRow(int rowID, int shift){
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                Buffer[i,rowID+shift] = Buffer[i,rowID];
            }
        }

        /// <summary>
        /// Перемещение буффера в общей сетке секторов влево на один сектор
        /// </summary>
        public void MoveLeft(){
            SaveColumn(Buffer.GetLength(0)-1);
            BufferStart[0]--;
            //Смещение всех колонок влево на 1 вправа налево
            for (int i = Buffer.GetLength(0)-2; i >= 0; i--)
            {
                ShiftColumn(i,1);
            }
            //загрузка самой левой колонки
            LoadColumn(0);
        }

        /// <summary>
        /// Перемещение буффера в общей сетке секторов влево на один сектор
        /// </summary>
        public void MoveUp(){
            SaveRow(0);
            BufferStart[1]++;
            //Смещение всех строк вправо на 1 слева направо
            for (int i = 1; i < Buffer.GetLength(1); i++)
            {
                ShiftRow(i,-1);
            }
            //загрузка самой правой колонки
            LoadRow(Buffer.GetLength(1)-1);
        }

        /// <summary>
        /// Перемещение буффера в общей сетке секторов влево на один сектор
        /// </summary>
        public void MoveRight(){
            SaveColumn(0);
            BufferStart[0]++;
            //Смещение всех колонок вправо на 1 слева направо
            for (int i = 1; i < Buffer.GetLength(0); i++)
            {
                ShiftColumn(i,-1);
            }
            //загрузка самой правой колонки
            LoadColumn(Buffer.GetLength(0)-1);
        }

        /// <summary>
        /// Перемещение буффера в общей сетке секторов влево на один сектор
        /// </summary>
        public void MoveDown(){
            SaveRow(Buffer.GetLength(1)-1);
            BufferStart[1]--;
            //Смещение всех строк влево на 1 вправа налево
            for (int i = Buffer.GetLength(1)-2; i >= 0; i--)
            {
                ShiftRow(i,1);
            }
            //загрузка самой левой колонки
            LoadRow(0);
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

        public readonly float CellSize;
        
        public SectorDict SuperSecDict = new SectorDict();

        public SectorLoader(float cellSize){
            CellSize = cellSize;
        }

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
                return new Sector(Data.ReadSector(ID),CellSize);
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