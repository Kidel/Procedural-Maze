using UnityEngine;

namespace Maze
{
    public static class Directions
    {
        public static readonly Vector2[] Cardinals = new Vector2[4]{
        new Vector2(0,-1),  // up
        new Vector2(0,1),   // down
        new Vector2(-1,0),  // left
        new Vector2(1,0)    // right
    };
        public static readonly Vector2 up = new Vector2(0, -1);
        public static readonly Vector2 down = new Vector2(0, 1);
        public static readonly Vector2 left = new Vector2(-1, 0);
        public static readonly Vector2 right = new Vector2(1, 0);
    }
}