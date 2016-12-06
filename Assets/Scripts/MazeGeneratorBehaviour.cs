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

    public GameObject Wall;
    public GameObject Floor;
    public GameObject Door;

    public GameObject Player;

    // Use this for initialization
    void Start()
    {
        if (StageWidth % 2 == 0)
            StageWidth++;
        if (StageHeight % 2 == 0)
            StageHeight++;

        mz = new MazeGenerator(StageWidth, StageHeight, NumberRoomTries, RoomExtraSize, ExtraConnectorChance, WindingPercent, RemoveDeadEnds);
        maze = mz.Generate();
        mz.PrintDebug();

        for (int y = 0; y < maze.GetLength(1); ++y)
        {
            for (int x = 0; x < maze.GetLength(0); ++x)
            {
                GameObject whatToSpawn = new GameObject();
                float size = Wall.transform.localScale.x;
                bool spawnFloor = false;
                if (maze[x, y] == Tiles.wall)
                {
                    whatToSpawn = Wall;
                }
                else if (maze[x, y] == Tiles.floor)
                {
                    whatToSpawn = Floor;
                }
                else if (maze[x, y] == Tiles.door)
                {
                    whatToSpawn = Door;
                }
                else if (maze[x, y] == Tiles.player)
                {
                    whatToSpawn = Player;
                    spawnFloor = true;
                }
                Instantiate(whatToSpawn, new Vector3(x * size, !spawnFloor ? size * (-1) : 0, y * size), new Quaternion());
                if (spawnFloor) Instantiate(Floor, new Vector3(x * size, size * (-1), y * size), new Quaternion());
            }
        }
    }
}
