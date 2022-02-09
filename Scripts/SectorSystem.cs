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
    }

    /// <summary>
    /// Класс для преобразования координат
    /// </summary>
    public class CoordTranslator{

        /// <summary>
        /// Размер одной ячейки сетки
        /// </summary>
        public readonly int CellSize = 0;

        /// <summary>
        /// Размер сектора
        /// </summary>
        public readonly int SectorSize = 0;

        /// <summary>
        /// количество секторов в суперсекторе
        /// </summary>
        public readonly int SupersectorSize = 0;

        /// <summary>
        /// Конструктор с задаваемыми параметрами
        /// </summary>
        /// <param name="cellSize">размер ячейки в сетке</param>
        /// <param name="sectorSize">количество ячеек в секторе</param>
        /// <param name="supersectorSize">количество секторов в суперсекторе</param>
        public CoordTranslator(int cellSize, int sectorSize, int supersectorSize){
            CellSize = cellSize;
            SectorSize = sectorSize;
            SupersectorSize = supersectorSize;
        }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="Other">Другой транслятор координат, с которого нужно сделать копию</param>
        public CoordTranslator(CoordTranslator Other){
            CellSize = Other.CellSize;
            SectorSize = Other.SectorSize;
            SupersectorSize = Other.SupersectorSize;
        }
    }

    /// <summary>
    /// Класс для подгрузки и управления загруженными секторами
    /// </summary>
    public class SectorBuffer : SectorLoader{
        
        /// <summary>
        /// Начальная точка буффера - верхняя левая точка в координатах сетки секторов
        /// </summary>
        /// <value></value>
        public int[] BufferStart = new int[] {0,0};
        /// <summary>
        /// Массив секторов буффера
        /// </summary>
        public Sector[,] Buffer = new Sector[3,3];

        /// <summary>
        /// Метод, возвращающий центральный сектор среди загруженных
        /// </summary>
        /// <returns></returns>
        public Sector GetCenterSector(){
            return Buffer[Buffer.GetLength(0)/2,Buffer.GetLength(1)/2];
        }

        /// <summary>
        /// Метод, возвращающий центральный сектор среди загруженных
        /// </summary>
        /// <returns></returns>
        public int[] GetCenterCoords(){
            return new int[] {Buffer.GetLength(0)/2, Buffer.GetLength(1)/2};
        }

        public string StringContent(){
            string Result = "";
            for (int i = 0; i < Buffer.GetLength(0); i++)
            {
                for (int j = 0; j < Buffer.GetLength(1); j++)
                {
                    Result += Buffer[i,j].StringContent()+"\n";
                }
            }
            return Result;
        }

        public int[] GetCenterGlobCoords(){
            int[] Center = GetCenterCoords();
            return new int[] {BufferStart[0] + Center[0], BufferStart[1] + Center[1]};
        }

        /// <summary>
        /// Метод, смещающий центр буффера к нужным координатам
        /// </summary>
        public void ShiftTo(Vector2 Pos){
            int ShiftX = SectorIncludes(Pos)[0] - GetCenterGlobCoords()[0];
            int ShiftY = SectorIncludes(Pos)[1] - GetCenterGlobCoords()[1];
            if(ShiftX > 0){
                for (int i = 0; i < ShiftX; i++)
                {
                    MoveRight();
                }
            } else {
                for (int i = ShiftX; i < 0; i++)
                {
                    MoveLeft();
                }
            }
            if(ShiftY > 0){
                for (int i = 0; i < ShiftY; i++)
                {
                    MoveUp();
                }
            } else {
                for (int i = ShiftY; i < 0; i++)
                {
                    MoveDown();
                }
            }
            GD.Print(" NewStart = ",BufferStart[0],BufferStart[1]);
            GD.Print(" NewCenter = ",GetCenterGlobCoords()[0],GetCenterGlobCoords()[1]);
            Vector2 Temp = SectorStartVector(BufferStart);
            GD.Print(" NewStartVector = ",Temp.x," ",Temp.y);
        }

        public SectorBuffer(float cellSize, int arraySize) : base(cellSize){
            Buffer = new Sector[arraySize,arraySize];
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
            GD.Print("SSC = ",SSC[0], SSC[1]);
            int[] ISC = InSectorCoords(Pos);
            GD.Print("ISC = ",ISC[0], ISC[1]);
            SuperSectorData Buff;
            //Проверка на наличие суперсектора в файле
            if(SuperSecDict.TryGetValue(SuperSecDict.KeyFromPos(SSC[0],SSC[1]),out Buff)){
                long ID = Buff.filePos;
                GD.Print(Pos[0], Pos[1], Buff.filePos);
                ID+=Data.GetIDShift(ISC[0],ISC[1]);
                return new Sector(Data.ReadSector(ID),CellSize);
            }
            else throw new Exception("There's no supersector with coords "+SSC[0]+" "+SSC[1]+" "+SuperSecDict.KeyFromPos(SSC[0],SSC[1]));
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
        /// Метод, вектор начала сектора, основываясь на его положении в сетке секторов
        /// </summary>
        /// <param name="SectorCoords"></param>
        /// <returns></returns>
        public Vector2 SectorStartVector(int[] SectorCoords){
            return new Vector2(Data.Translator.SectorSize*CellSize*SectorCoords[0],
            Data.Translator.SectorSize*CellSize*SectorCoords[1]);
        }

        /// <summary>
        /// Метод, возвращающий координаты сектора, включающего данные координаты, сетке секторов.
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public int[] SectorIncludes(Vector2 Pos){
            return new int[] {
                (int)Math.Truncate(Pos.x/(Data.Translator.SectorSize*CellSize)),
                (int)Math.Truncate(Pos.y/(Data.Translator.SectorSize*CellSize))
                };
        }

        /// <summary>
        /// Метод получения координат суперсектора на основе координат сектора
        /// </summary>
        /// <param name="SectGlobCoords">положение сектора в глобальной сетке секторов, суперсектор которого надо определить</param>
        /// <returns></returns>
        public int[] SupSecCoords(int[] SectGlobCoords){
            return new int[] {
                (int)Math.Floor((decimal)SectGlobCoords[0]/Data.Translator.SupersectorSize),
                (int)Math.Floor((decimal)SectGlobCoords[1]/Data.Translator.SupersectorSize)
                };
        }

        /// <summary>
        /// Метод преобразовывающий глобальные координаты сектора в сетке секторов в координаты сектора внутри суперсектора
        /// </summary>
        /// <param name="SectGlobCoords"></param>
        /// <returns></returns>
        public int[] InSectorCoords(int[] SectGlobCoords){
            int [] SSC = SupSecCoords(SectGlobCoords);
            return new int[] {
                (int)SectGlobCoords[0] - SSC[0] * Data.Translator.SupersectorSize,
                (int)SectGlobCoords[1] - SSC[1] * Data.Translator.SupersectorSize
                };
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
        public int[] GetCellInSector(Vector2 Pos){
            return new int[] {
                (int)Math.Truncate((decimal)(Pos.x/CellSize)),
                (int)Math.Truncate((decimal)(Pos.y/CellSize))
                };
        }
    
    }
}