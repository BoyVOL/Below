using Godot;

namespace MapSystem
{

    /// <summary>
    /// Контроллер коллизий
    /// </summary>
    public class CollController{
        
    }

    /// <summary>
    /// Абстрактный класс для определения сетки коллизий с глобальной картой
    /// </summary>
    public class CollGrid{

        public Sector[,] CurrentSector = new Sector[3,3];

        /// <summary>
        /// Текущее положение сетки коллизии в секторе.
        /// </summary>
        /// <returns></returns>
        public Transform2D Transform = new Transform2D();

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
        public virtual void CalcCollision(int Xid, int Yid){

        }

        /// <summary>
        /// Метод, возвращающий координаты по указанным индексам
        /// </summary>
        /// <param name="Xid"></param>
        /// <param name="Yid"></param>
        /// <returns></returns>
        public Vector2 GetCoords(int Xid, int Yid){
            Vector2 Result = Transform * new Vector2(Xid*CellSize,Yid*CellSize);
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

    public class TestGrid : CollGrid{
        
        public override void CalcCollision(int Xid, int Yid){

        }
    }
}