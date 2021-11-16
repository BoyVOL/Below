using Godot;
using System;
using System.IO;

namespace MapSystem{

    /// <summary>
    /// Родительский класс для классов работы с байтовыми файлами
    /// </summary>
    public class ByteFile{

        byte[] Buffer;

        long FilePos;

        protected FileStream File = null;


        public void OpenFile(string Filepath){
            File = new FileStream(Filepath,FileMode.OpenOrCreate, 
            FileAccess.ReadWrite, FileShare.None);
            FilePos = 0;
        }

        public void ReloadFile(string Filepath){
            File = new FileStream(Filepath,FileMode.CreateNew, 
            FileAccess.ReadWrite, FileShare.None);
            FilePos = 0;
        }
        
        public void CloseFile(){
            File.Close();
        }

        /// <summary>
        /// Перемещение курсора на нужное положение в файле
        /// </summary>
        /// <param name="pos"></param>
        public void GetToPos(int pos){
            FilePos = File.Seek(pos,SeekOrigin.Begin);
        }

        public byte[] ReadByteArray(int Length){
            Buffer = new byte[Length];
            File.Read(Buffer,0,Length);
            return Buffer;
        }

        public void WriteByteArray(byte[] Data){
            File.Write(Data,0,Data.Length);
        }
    }

    /// <summary>
    /// Класс для создания и сохранения глобальной карты
    /// </summary>
    public class CellMapFile : ByteFile{

        public readonly int SectorSize;

        public readonly int SectorByteSize;

        public readonly int CellByteLength;

        public CellMapFile(int sectorSize){
            SectorSize = sectorSize;
            CellByteLength =MapCell.ByteLength;
            SectorByteSize = sectorSize*sectorSize*CellByteLength;
        }

        /// <summary>
        /// Метод для чтения секторов из файла
        /// </summary>
        /// <returns></returns>
        public MapCell[,] ReadSector(){
            byte[] Temp = new byte[CellByteLength];
            MapCell[,] Result = new MapCell[SectorSize,SectorSize];
            GetToPos(0);
            for (int i = 0; i < SectorSize; i++)
            {
                for (int j = 0; j < SectorSize; j++)
                {
                    File.Read(Temp,0,CellByteLength);
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
            GetToPos(0);
            for (int i = 0; i < SectorSize; i++)
            {
                for (int j = 0; j < SectorSize; j++)
                {
                    Temp = sector[i,j].ToByte();
                    File.Write(Temp,0,CellByteLength);
                }
            }
        }
    }
}