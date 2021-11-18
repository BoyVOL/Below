using Godot;
using System;
using System.IO;

namespace MapSystem{

    /// <summary>
    /// Родительский класс для классов работы с байтовыми файлами
    /// </summary>
    public class ByteFile{

        byte[] Buffer;

        protected FileStream File = null;

        /// <summary>
        /// Метод, открывающий файл для чтения/записи
        /// </summary>
        /// <param name="Filepath">абсолютный путь до файла</param>
        public void OpenFile(string Filepath){
            File = new FileStream(Filepath,FileMode.OpenOrCreate, 
            FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Метод, пересоздающий нужный файл
        /// </summary>
        /// <param name="Filepath">абсолютный путь до файла</param>
        public void ReloadFile(string Filepath){
            File = new FileStream(Filepath,FileMode.CreateNew, 
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
        public void GetToPos(int pos){
            File.Position = pos;
        }

        public bool IsEOF(){
            return File.Position >= File.Length;
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
        public MapCell[,] ReadSector(int id){
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
        public void WriteSector(MapCell[,] sector, int id){
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

        public void WriteSupersector(MapCell [,][,] data, int id){
            GetToPos(id);
            for (int i = 0; i < SupersectorSize; i++)
            { 
                for (int j = 0; j < SupersectorSize; j++)
                {
                    WriteSector(data[i,j],id+i*SupersectorSize+j);
                }
            }
        }

        public MapCell [,][,] ReadSupersector(int id){
            MapCell [,][,] Result = new MapCell[SupersectorSize, SupersectorSize][,];
            for (int i = 0; i < SupersectorSize; i++)
            { 
                for (int j = 0; j < SupersectorSize; j++)
                {
                    Result[i,j] = ReadSector(id+i*SupersectorSize+j);
                }
            }
            return Result;
        }
    }

    /// <summary>
    /// Структура, отвечающая за описание подгружаемых секторов
    /// </summary>
    public struct SectorRecord{
        
        /// <summary>
        /// Координата x сектора
        /// </summary>
        public int x;

        /// <summary>
        /// Координата y сектора
        /// </summary>
        public int y;

        /// <summary>
        /// Размер записи в битах
        /// </summary>
        public const int ByteLength = 4+4+4;

        /// <summary>
        /// Положение сектора в файле
        /// </summary>
        public int filePos;

        public SectorRecord(byte [] data){
            x = BitConverter.ToInt32(data,0);
            y = BitConverter.ToInt32(data,4);
            filePos = BitConverter.ToInt32(data,8);
        }

        public byte[] ToByte(){
            byte[] Result = new byte[ByteLength];
            BitConverter.GetBytes(x).CopyTo(Result,0);
            BitConverter.GetBytes(y).CopyTo(Result,4);
            BitConverter.GetBytes(filePos).CopyTo(Result,8);
            return Result;
        }
    }
    
    /// <summary>
    /// Класс для записи файла с аллокацией всех секторов карты в файле
    /// </summary>
    public class SectorRecordsFile : ByteFile{
        public SectorRecord[] ReadRecords(){
            SectorRecord[] Result = new SectorRecord[File.Length / SectorRecord.ByteLength];
            byte[] Buff = new byte[SectorRecord.ByteLength];
            GetToPos(0);
            for (int i = 0; i < Result.Length; i++)
            {
                File.Read(Buff,0,SectorRecord.ByteLength);
                Result[i] = new SectorRecord(Buff);
            }
            return Result;
        }

        public void WriteRecords(SectorRecord[] data){
            byte[] Buff = new byte[SectorRecord.ByteLength];
            GetToPos(0);
            for (int i = 0; i < data.Length; i++)
            {
                Buff = data[i].ToByte();
                File.Write(Buff,0,SectorRecord.ByteLength);
            }
        }
    }
}