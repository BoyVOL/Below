using Godot;
using System.Collections.Generic;
using System;

namespace MapSystem{

    /// <summary>
    /// Глобальный контроллер мира игры
    /// </summary>
    public class WorldController{

        public SectorBuffer Buffer;

        void InitBuffer(string SecDataFileName, string SupSectDictFileName, float cellSize, int BufferSize, int SectorSize){
            Buffer = new SectorBuffer(cellSize,BufferSize);
            SectorRecordsFile Sectors = new SectorRecordsFile();
            SupersectorFile SupSectors = new SupersectorFile(SectorSize,SectorSize);
            Sectors.FilePath = SupSectDictFileName;
            SupSectors.FilePath = SecDataFileName;
            Buffer.Data = SupSectors;
            Buffer.SuperSecDict.Records = Sectors;
        }

        void ResizeBuffer(){

        }

        public WorldController(string SecDataFileName, string SupSectDictFileName, float cellSize, int BufferSize, int SectorSize){
            InitBuffer(SecDataFileName, SupSectDictFileName, cellSize, BufferSize, SectorSize);
        }

        /// <summary>
        /// Метод для инициализации мира игры
        /// </summary>
        public void Init(int Startx, int StartY){
            Buffer.Data.OpenFile();
            Buffer.SuperSecDict.Records.OpenFile();
            Buffer.SuperSecDict.LoadDictionary();
            Buffer.LoadBuffer();
            Buffer.BufferStart = new int[] {Startx,StartY};
        }
    }

}