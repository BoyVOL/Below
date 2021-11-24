using Godot;
using System;

namespace MapSystem{
    /// <summary>
    /// Структура данных ячейки
    /// </summary>
    public struct MapCell{

        /// <summary>
        /// Идентификатор, какой айди должен быть для отрисовки данной ячейки
        /// </summary>
        public byte TileID;

        public const int ByteLength = 1;
        
        public MapCell(byte[] data){
            TileID = data[0];
        }

        public byte[] ToByte(){
            byte[] Result = new byte[ByteLength];
            Result[0] = TileID;
            return Result;
        }
    }

    /// <summary>
    /// Структура, отвечающая за описание подгружаемых секторов
    /// </summary>
    public struct SuperSectorRecord{
        
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
        public const int ByteLength = 4+4+SuperSectorData.ByteLength;

        /// <summary>
        /// Положение сектора в файле
        /// </summary>
        public SuperSectorData Data;

        public SuperSectorRecord(byte [] data){
            x = BitConverter.ToInt32(data,0);
            y = BitConverter.ToInt32(data,4);
            byte [] Temp = new byte[SuperSectorData.ByteLength];
            Array.Copy(data,8,Temp,0,SuperSectorData.ByteLength);
            Data = new SuperSectorData(Temp);
        }

        public byte[] ToByte(){
            byte[] Result = new byte[ByteLength];
            BitConverter.GetBytes(x).CopyTo(Result,0);
            BitConverter.GetBytes(y).CopyTo(Result,4);
            Data.ToByte().CopyTo(Result,8);
            return Result;
        }
    }

    /// <summary>
    /// Структура, содержащая данные об индексах суперсектора
    /// </summary>
    public struct SuperSectorData{

        /// <summary>
        /// Размер записи в битах
        /// </summary>
        public const int ByteLength = 8;
        

        /// <summary>
        /// Положение сектора в файле
        /// </summary>
        public long filePos;

        public SuperSectorData(byte [] data){
            filePos = BitConverter.ToInt32(data,0);
        }

        public byte[] ToByte(){
            byte[] Result = new byte[ByteLength];
            BitConverter.GetBytes(filePos).CopyTo(Result,0);
            return Result;
        }
    }
    
}