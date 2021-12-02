using Godot;

namespace MapSystem
{
    /// <summary>
    /// Абстрактный класс для определения сетки коллизий
    /// </summary>
    public abstract class CollGrid{

        public Vector2 Position;

        public float Rotation;

        /// <summary>
        /// Массив размеров сетки коллизий
        /// </summary>
        public int[] size = new int[2];

        float CellSize;

        /// <summary>
        /// Метод для проверки одного столкновения по указанным индексам
        /// </summary>
        /// <param name="Xid"></param>
        /// <param name="Yid"></param>
        public abstract void CalcCollision(int Xid, int Yid);

        /// <summary>
        /// Метод, возвращающий глобальные координаты по указанным индексам
        /// </summary>
        /// <param name="Xid"></param>
        /// <param name="Yid"></param>
        /// <returns></returns>
        public Vector2 GetCoords(int Xid, int Yid){
            Vector2 Result = new Vector2(Position.x+Xid*CellSize, Position.y+Yid*CellSize).Rotated(Rotation);
            return Result;
        }

        /// <summary>
        /// Метод для проверки столкновений по всей сетке
        /// </summary>
        public void CalcAllCollisions(){
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    CalcCollision(i,j);
                }
            }
        }
    }
}