using Godot;
using System.IO;

namespace MapSystem{

    /// <summary>
    /// Родительский класс для классов работы с байтовыми файлами
    /// </summary>
    public class ByteFile{

        public string FilePath = "";

        protected FileStream File = null;

        /// <summary>
        /// Метод, открывающий файл для чтения/записи
        /// </summary>
        /// <param name="Filepath">абсолютный путь до файла</param>
        public void OpenFile(){
            File = new FileStream(FilePath,FileMode.OpenOrCreate, 
            FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Метод, пересоздающий нужный файл
        /// </summary>
        /// <param name="Filepath">абсолютный путь до файла</param>
        public void ReloadFile(){
            File.Close();
            System.IO.File.WriteAllText(FilePath,string.Empty);
            File = new FileStream(FilePath,FileMode.OpenOrCreate, 
            FileAccess.ReadWrite, FileShare.None);
        }
        
        /// <summary>
        /// Метод, закрывающий файл
        /// </summary>
        public void CloseFile(){
            File.Close();
        }

        /// <summary>
        /// Перемещение курсора на нужное положение в файле
        /// </summary>
        /// <param name="pos"></param>
        public void GetToPos(long pos){
            File.Position = pos;
        }

        public long CurrentPos(){
            return File.Position;
        }
        public bool IsEOF(){
            return File.Position >= File.Length;
        }

        /// <summary>
        /// Возвращает общую длину файла
        /// </summary>
        /// <returns></returns>
        public long GetLength(){
            return File.Length;
        }
    }

    /// <summary>
    /// Класс для создания и сохранения глобальной карты
    /// </summary>
    public class CellMapFile : ByteFile{

        public readonly int SectorSize;

        public readonly int SectorByteSize;

        public CellMapFile(int sectorSize){
            SectorSize = sectorSize;
            SectorByteSize = sectorSize*sectorSize*MapCell.ByteLength;
        }

        /// <summary>
        /// Метод для чтения секторов из файла
        /// </summary>
        /// <param name="id">индекс, с которого производится чтение</param>
        /// <returns></returns>
        public MapCell[,] ReadSector(long id){
            byte[] Temp = new byte[MapCell.ByteLength];
            MapCell[,] Result = new MapCell[SectorSize,SectorSize];
            GetToPos(id);
            for (int i = 0; i < SectorSize; i++)
            {
                for (int j = 0; j < SectorSize; j++)
                {
                    File.Read(Temp,0,MapCell.ByteLength);
                    Result[i,j] = new MapCell(Temp);
                }
            }
            return Result;
        }

        /// <summary>
        /// Метод для записи секторов в файл
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="id">Индекс, с которого идёт чтение</param>
        public void WriteSector(MapCell[,] sector, long id){
            byte[] Temp = new byte[MapCell.ByteLength];
            GetToPos(id);
            for (int i = 0; i < SectorSize; i++)
            {
                for (int j = 0; j < SectorSize; j++)
                {
                    Temp = sector[i,j].ToByte();
                    File.Write(Temp,0,MapCell.ByteLength);
                }
            }
        }
    }

    /// <summary>
    /// Класс для загрузки групп секторов - (суперсекторв) в виде массива массивов
    /// </summary>
    public class SupersectorFile : CellMapFile{

        /// <summary>
        /// Размер суперсектора
        /// </summary>
        public readonly int SupersectorSize;

        private SupersectorFile(int sectorSize): base(sectorSize){
        }

        /// <summary>
        /// конструктор с двумя параметрами, обозначающими размеры секторов и суперсекторов
        /// </summary>
        /// <param name="sectorSize">размер стороны сектора</param>
        /// <param name="supersectorSize">размер стороны суперсектора</param>
        /// <returns></returns>
        public SupersectorFile(int sectorSize, int supersectorSize): base(sectorSize){
            SupersectorSize = supersectorSize;
        }

        /// <summary>
        /// Преобразовывает координаты сектора в суперсекторе в смещение индекса относительно начала записи сектора в файле
        /// </summary>
        /// <param name="x">координата x сектора</param>
        /// <param name="y">координата y сектора</param>
        /// <returns>индекс чтения/записи в файле</returns>
        public long GetIDShift(int x, int y){
            return (x*SupersectorSize+y)*SectorByteSize;
        }

        /// <summary>
        /// Записывает массив массивов клеток в файл
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        public void WriteSupersector(MapCell [,][,] data, long id){
            GetToPos(id);
            for (int i = 0; i < SupersectorSize; i++)
            { 
                for (int j = 0; j < SupersectorSize; j++)
                {
                    WriteSector(data[i,j],id+GetIDShift(i,j));
                }
            }
        }

        /// <summary>
        /// Читает из файла по указанному индексу и возвращает их, преобразовав в массив массивов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MapCell [,][,] ReadSupersector(long id){
            MapCell [,][,] Result = new MapCell[SupersectorSize, SupersectorSize][,];
            for (int i = 0; i < SupersectorSize; i++)
            { 
                for (int j = 0; j < SupersectorSize; j++)
                {
                    Result[i,j] = ReadSector(id+GetIDShift(i,j));
                }
            }
            return Result;
        }
    }

    /// <summary>
    /// Класс для записи файла с аллокацией всех секторов карты в файле
    /// </summary>
    public class SectorRecordsFile : ByteFile{

        /// <summary>
        /// Читает все записи из файла
        /// </summary>
        /// <returns></returns>
        public SuperSectorRecord[] ReadRecords(){
            SuperSectorRecord[] Result = new SuperSectorRecord[File.Length / SuperSectorRecord.ByteLength];
            byte[] Buff = new byte[SuperSectorRecord.ByteLength];
            GetToPos(0);
            for (int i = 0; i < Result.Length; i++)
            {
                File.Read(Buff,0,SuperSectorRecord.ByteLength);
                Result[i] = new SuperSectorRecord(Buff);
            }
            return Result;
        }

        /// <summary>
        /// полностью стирает контент файла и перезаполняет его новым массивом записей
        /// </summary>
        /// <param name="data"></param>
        public void WriteRecords(SuperSectorRecord[] data){
            byte[] Buff = new byte[SuperSectorRecord.ByteLength];
            ReloadFile();
            GetToPos(0);
            for (int i = 0; i < data.Length; i++)
            {
                Buff = data[i].ToByte();
                File.Write(Buff,0,SuperSectorRecord.ByteLength);
            }
        }
    }
}