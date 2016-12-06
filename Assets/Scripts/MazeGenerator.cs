using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The random dungeon generator.
///
/// Starting with a stage of solid walls, it works like so:
///
/// 1. Place a number of randomly sized and positioned rooms. If a room
///    overlaps an existing room, it is discarded. Any remaining rooms are
///    carved out.
/// 2. Any remaining solid areas are filled in with mazes. The maze generator
///    will grow and fill in even odd-shaped areas, but will not touch any
///    rooms.
/// 3. The result of the previous two steps is a series of unconnected rooms
///    and mazes. We walk the stage and find every tile that can be a
///    "connector". This is a solid tile that is adjacent to two unconnected
///    regions.
/// 4. We randomly choose connectors and open them or place a door there until
///    all of the unconnected regions have been joined. There is also a slight
///    chance to carve a connector between two already-joined regions, so that
///    the dungeon isn't single connected.
/// 5. The mazes will have a lot of dead ends. Finally, we remove those by
///    repeatedly filling in any open tile that's closed on three sides. When
///    this is done, every corridor in a maze actually leads somewhere.
///
/// The end result of this is a multiply-connected dungeon with rooms and lots
/// of winding corridors.
/// </summary>
public class MazeGenerator
{
    public char[,] Maze { get; private set; }

    public int StageWidth { get; set; }
    public int StageHeight { get; set; }

    public int NumberRoomTries { get; set; }

    public int NumberOfEnemies { get; set; }

    public bool RemoveDeadEnds { get; set; }

    /// <summary>
    /// The inverse chence of adding a connector between two regions that have
    /// already been joined. Increasing this leads to more loosely connected dungeons
    /// </summary>
    public int ExtraConnectorChance { get; set; }

    /// <summary>
    /// Increasing this allows rooms to be larger
    /// </summary>
    public int RoomExtraSize { get; set; }

    public int WindingPercent { get; set; }

    private List<Rect> _rooms;

    public MazeGenerator(int stageWidth = 51, int stageHeight = 51, int numberRoomTries = 30, int roomExtraSize=0, int extraConnectorChance = 20, int windingPercent = 0, bool removeDeadEnds = true, int numberOfEnemies = 10)
    {
        StageHeight = stageHeight;
        StageWidth = stageWidth;
        NumberRoomTries = numberRoomTries;
        RoomExtraSize = roomExtraSize;
        RemoveDeadEnds = removeDeadEnds;
        WindingPercent = windingPercent;
        NumberOfEnemies = numberOfEnemies;
        ExtraConnectorChance = extraConnectorChance;
        Maze = new char[stageWidth, stageHeight];
        _rooms = new List<Rect>();
    }

    public char[,] Generate()
    {
        if (StageWidth % 2 == 0 || StageHeight % 2 == 0)
        {
            throw new System.Exception("The stage must be odd-sized.");
        }

        _fill(Tiles.wall);

        // Add the rooms
        _addRooms();

        // Fill in all of the empty space with mazes.
        for (int y = 1; y < StageHeight; y += 2)
        {
            for (int x = 1; x < StageWidth; x += 2)
            {
                _growMaze(x, y);
            }
        }

        _connectRegions();
        if(RemoveDeadEnds)
            _removeDeadEnds();

        _addPlayer();
        _addEnemies();

        return Maze;
    }

    private void _addRooms()
    {
        for (int i = 0; i < NumberRoomTries; i++)
        {
            // Pick a random room size. The funny math here does two things:
            // - It makes sure rooms are odd-sized to line up with maze.
            // - It avoids creating rooms that are too rectangular: too tall and
            //   narrow or too wide and flat.
            // TODO: This isn't very flexible or tunable. Do something better here.
            int size = (int)Random.Range(1, 3 + RoomExtraSize) * 2 + 1;
            int rectangularity = (int)Random.Range(0, 1 + size / 2) * 2;
            var width = size;
            var height = size;
            if (Random.Range(0, 1) > 0.5f)
            {
                width += rectangularity;
            }
            else
            {
                height += rectangularity;
            }

            int x = (int)Random.Range(0, (StageWidth - width) / 2) * 2 + 1;
            int y = (int)Random.Range(0, (StageHeight - height) / 2) * 2 + 1;

            Rect room = new Rect(x, y, width, height);

            bool overlaps = false;
            for (int j = 0; j < _rooms.Count; j++)
            {
                if (room.Overlaps(_rooms[j]))
                {
                    overlaps = true;
                    break;
                }
            }
            if (overlaps) continue;

            _rooms.Add(room);

            for (int pos = 0; pos < width * height; pos++)
            {
                _carve((pos % width) + x, (int)(pos / width) + y);
            }
        }
    }

    /// <summary>
    /// Implementation of the "growing tree" algorithm from here:
    /// http://www.astrolog.org/labyrnth/algrithm.htm.
    /// </summary>
    private void _growMaze(int start_x, int start_y)
    {
        LinkedList<Vector2> cells = new LinkedList<Vector2>();
        Vector2 lastDirection = new Vector2();

        _carve(start_x, start_y, Tiles.corridor);

        cells.AddLast(new Vector2(start_x, start_y));

        int loopAvoidance = 0;
        while (cells.Count != 0 && loopAvoidance < 10000)
        {
            loopAvoidance++;
            Vector2 cell = cells.Last.Value;

            // See which adjacent cells are open.
            List<Vector2> unmadeCells = new List<Vector2>();

            for (int i = 0; i < Directions.Cardinals.Length; i++)
            {
                Vector2 dir = Directions.Cardinals[i];
                if (_canCarve(cell + dir) && _canCarve(cell + dir * 2)) unmadeCells.Add(dir);
            }

            if (unmadeCells.Count != 0)
            {
                // Based on how "windy" passages are, try to prefer carving in the
                // same direction.
                Vector2 dir;
                if (unmadeCells.Contains(lastDirection) && Random.Range(0, 100) < 100 - WindingPercent)
                {
                    dir = lastDirection;
                }
                else
                {
                    int randomIndex = Random.Range(0, unmadeCells.Count - 1);
                    dir = unmadeCells[randomIndex];
                }

                _carve((int)(cell + dir).x, (int)(cell + dir).y, Tiles.corridor);
                _carve((int)(cell + dir * 2).x, (int)(cell + dir * 2).y, Tiles.corridor);

                cells.AddLast(cell + dir * 2);
                lastDirection = dir;
            }
            else
            {
                // No adjacent uncarved cells.
                cells.RemoveLast();

                // This path has ended.
                lastDirection = new Vector2();
            }
        }
    }

    private bool _canCarve(Vector2 nextCell)
    {
        return ((int)nextCell.x > 0 && (int)nextCell.y > 0 && (int)nextCell.x < StageWidth && (int)nextCell.y < StageHeight && Maze[(int)nextCell.x, (int)nextCell.y] == Tiles.wall);
    }

    private void _carve(int position_x, int position_y, char type = '0')
    {
        if (type == '0') type = Tiles.floor;
        try
        {
            _setTile(position_x, position_y, type);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            Debug.Log(position_x + "/" + Maze.GetLength(0));
            Debug.Log(position_y + "/" + Maze.GetLength(1));
        }
    }

    private void _setTile(int x, int y, char type)
    {
        Maze[x, y] = type;
    }

    private void _setTile(Vector2 pos, char type)
    {
        _setTile((int)pos.x, (int)pos.y, type);
    }

    private void _fill(char type)
    {
        for (int i = 0; i < StageWidth; i++)
            for (int j = 0; j < StageHeight; j++)
                Maze[i, j] = type;
    }

    private void _connectRegions()
    {
        // For each rectangle find walls that has floor behind
        for (int i = 0; i < _rooms.Count; i++)
        {
            int connections = 0;
            int lastSide = -1;
            // until we have at least 2 connections for that rectangle
            int doorTries = 0;
            int moreDoors = Random.Range(0, 100) > 100 - ExtraConnectorChance ? 1 : 0;
            while (connections < 2 + moreDoors && doorTries < 100)
            {
                doorTries++;
                // Pick a random side
                int side = Random.Range(0, 15) % 4;
                if (side == lastSide) side = (side + Random.Range(0,3)) % 4; // Priority to different side then last one
                Line line;
                Vector2 direction;
                if (side == 0)
                {
                    line = new Line(_rooms[i].x + 1, _rooms[i].y, _rooms[i].xMax - 1, _rooms[i].y);
                    direction = Directions.up;
                }
                else if (side == 1)
                {
                    line = new Line(_rooms[i].xMax + 1, _rooms[i].y + 2, _rooms[i].xMax + 1, _rooms[i].yMax);
                    direction = Directions.right;
                }
                else if (side == 2)
                {
                    line = new Line(_rooms[i].x + 2, _rooms[i].yMax + 1, _rooms[i].xMax, _rooms[i].yMax + 1);
                    direction = Directions.down;
                }
                else // if (side == 3)
                {
                    line = new Line(_rooms[i].x, _rooms[i].y + 1, _rooms[i].x, _rooms[i].yMax - 1);
                    direction = Directions.left;
                }

                // Pick a random point in the line perpendicular to direction
                int randomX = Random.Range((int)line.Start.x, (int)line.End.x) - 1;
                int randomY = Random.Range((int)line.Start.y, (int)line.End.y) - 1;

                // Debug
                // Maze[randomX, randomY] = side.ToString()[0];

                // see if point + direction is floor, if it is, place the door and increment connections
                if (randomY + direction.y >= 0 && randomX + direction.x >= 0 &&
                    randomY + direction.y < Maze.GetLength(1) && randomX + direction.x < Maze.GetLength(0) &&
                    _noDoorsAround(randomX, randomY) &&
                    Maze[randomX + (int)direction.x, randomY + (int)direction.y] == Tiles.floor)
                {
                    Maze[randomX, randomY] = Tiles.door;
                    connections++;
                    if(connections >= 2) lastSide = -1;
                }

            }
        }
    }

    private void _addPlayer()
    {
        // Pick a random room
        Rect randomRoom = _rooms[Random.Range(0, _rooms.Count)];
        // Pick a random floor tile in the room, 2 tiles from the wall
        Vector2 pos = new Vector2(Random.Range(randomRoom.x + 2, randomRoom.xMax - 2), Random.Range(randomRoom.y + 2, randomRoom.yMax - 2));

        // set player position
        if(_getTile(pos) == Tiles.floor)
            _setTile(pos, Tiles.player);
    }

    private void _addEnemies()
    {
        for (int i = 0; NumberOfEnemies > 0 && i < 1000; i++)
        {
            // Pick a random room
            Rect randomRoom = _rooms[Random.Range(0, _rooms.Count)];
            // Pick a random floor tile in the room, 2 tiles from the wall
            Vector2 pos = new Vector2(Random.Range(randomRoom.x + 2, randomRoom.xMax - 2), Random.Range(randomRoom.y + 2, randomRoom.yMax - 2));

            // set player position
            if (_getTile(pos) == Tiles.floor)
            {
                _setTile(pos, Tiles.enemy);
                NumberOfEnemies--;
            }
        }
    }

    private bool _noDoorsAround(int x, int y)
    {
        if (Maze[x, y] == Tiles.door)
        {
            return false;
        }
        for (int i = 0; i < Directions.Cardinals.Length; i++)
        {
            if (x + (int)Directions.Cardinals[i].x >= 0 && y + (int)Directions.Cardinals[i].y >= 0 &&
               x + (int)Directions.Cardinals[i].x < Maze.GetLength(0) && y + (int)Directions.Cardinals[i].y < Maze.GetLength(1) &&
               Maze[x + (int)Directions.Cardinals[i].x, y + (int)Directions.Cardinals[i].y] == Tiles.door)
            {
                return false;
            }
        }
        return true;
    }

    private char _getTile(Vector2 pos)
    {
        return Maze[(int)pos.x, (int)pos.y];
    }

    private void _removeDeadEnds()
    {
        var done = false;

        while (!done)
        {
            done = true;

            for (int i = 0; i < StageHeight*StageWidth; i++)
            {
                Vector2 pos = new Vector2(i % StageWidth, (int)(i/StageWidth));
                if (_getTile(pos) == Tiles.wall) continue;

                // If it only has one exit, it's a dead end.
                var exits = 0;
                for (int j = 0; j < Directions.Cardinals.Length; j++)
                {
                    Vector2 dir = Directions.Cardinals[j];
                    if (_getTile(pos + dir) != Tiles.wall) exits++;
                }

                if (exits != 1) continue;

                done = false;
                _setTile(pos, Tiles.wall);
            }
        }
    }

    public void PrintDebug()
    {
        var matrix = Maze;
        string print = "";

        for (int y = 0; y < matrix.GetLength(1); ++y)
        {
            for (int x = 0; x < matrix.GetLength(0); ++x)
            {
                print += matrix[x,y];
            }
            print += "\n";
        }

        Debug.Log(print);
    }
}