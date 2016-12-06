using UnityEngine;

public class MazeGeneratorBehaviour : MonoBehaviour
{

    private MazeGenerator mz;
    private char[,] maze;

    public int StageWidth = 51;
    public int StageHeight = 51;
    public int NumberRoomTries = 30;
    public int RoomExtraSize = 0;
    public int ExtraConnectorChance = 20;
    public int WindingPercent = 0;
    public bool RemoveDeadEnds = true;
    public int NumberOfEnemies = 10;

    public GameObject Wall;
    public GameObject Floor;
    public GameObject Door;
    public GameObject Enemy;

    public GameObject Player;

    // Use this for initialization
    void Start()
    {
        if (StageWidth % 2 == 0)
            StageWidth++;
        if (StageHeight % 2 == 0)
            StageHeight++;

        mz = new MazeGenerator(StageWidth, StageHeight, NumberRoomTries, RoomExtraSize, ExtraConnectorChance, WindingPercent, RemoveDeadEnds, NumberOfEnemies);
        maze = mz.Generate();
        mz.PrintDebug();

        for (int y = 0; y < maze.GetLength(1); ++y)
        {
            for (int x = 0; x < maze.GetLength(0); ++x)
            {
                GameObject whatToSpawn = null;
                bool spawn = false;
                float size = Wall.transform.localScale.x;
                bool spawnFloor = false;
                if (maze[x, y] == Tiles.wall)
                {
                    whatToSpawn = Wall;
                    spawn = true;
                }
                else if (maze[x, y] == Tiles.floor)
                {
                    whatToSpawn = Floor;
                    spawn = true;
                }
                else if (maze[x, y] == Tiles.door)
                {
                    whatToSpawn = Door;
                    spawn = true;
                }
                else if (maze[x, y] == Tiles.enemy)
                {
                    whatToSpawn = Enemy;
                    spawnFloor = true;
                    spawn = true;
                }
                else if (maze[x, y] == Tiles.player)
                {
                    whatToSpawn = Player;
                    spawnFloor = true;
                    spawn = true;
                }
                else
                {
                    spawn = false;
                    spawnFloor = false;
                }

                if (spawn) Instantiate(whatToSpawn, new Vector3(x * size, !spawnFloor ? size * (-1) : 0, y * size), new Quaternion());
                if (spawn && spawnFloor) Instantiate(Floor, new Vector3(x * size, size * (-1), y * size), new Quaternion());
            }
        }
    }
}
