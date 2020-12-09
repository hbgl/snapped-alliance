using UnityEngine;
using System.Collections;
using Assets.Scripts.Pipeline;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using System;
using Unity.Mathematics;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class Pathfinder : MonoBehaviour, ILifecycleObject
    {
        public GameObject DebugSpherePrefab;

        public bool DebugPath = true;

        public NavNode[,] Grid;

        public Tile[,] Tiles;

        public Tile[,][] Neighbors;

        public GameObject Player;

        private List<int2> Path;

        private List<Vector3> Points;

        private FastPriorityQueue<Tile> OpenSet;

        private Dictionary<Tile, bool> Touched;

        public NavNode PathStart;

        private List<GameObject> DebugPathObjects;

        public void OnSceneLoaded()
        {

        }

        public void OnEnable()
        {
            var navNodes = new List<NavNode>();
            var gameObjects = this.gameObject.scene.GetRootGameObjects();
            foreach (var gameObject in gameObjects)
            {
                var navNode = gameObject.GetComponent<NavNode>();
                if (navNode != null)
                {
                    navNodes.Add(navNode);
                }
            }


            var width = navNodes.Max(n => n.X) + 1;
            var height = navNodes.Max(n => n.Y) + 1;

            var grid = new NavNode[height, width];
            var tiles = new Tile[height, width];
            var neighbors = new Tile[height, width][];

            foreach (var navNode in navNodes)
            {
                var y = navNode.Y;
                var x = navNode.X;
                ref var cell = ref grid[y, x];
                if (cell != null)
                {
                    Debug.Log($"Cell ({x},{y}) is already occupied.]");
                }
                else
                {
                    cell = navNode;
                    tiles[y, x] = new Tile(x, y);
                }
            }

            foreach (var navNode in navNodes)
            {
                var neighborGameObjects = navNode.Neighbors;
                var neighborLength = neighborGameObjects.Length;
                ref var neighborTiles = ref neighbors[navNode.Y, navNode.X];
                neighborTiles = new Tile[neighborLength];
                for (var i = 0; i < neighborLength; i++)
                {
                    var neighborNavNode = neighborGameObjects[i].GetComponent<NavNode>();
                    neighborTiles[i] = tiles[neighborNavNode.Y, neighborNavNode.X];
                }
            }

            Grid = grid;
            Tiles = tiles;
            Neighbors = neighbors;
            OpenSet = new FastPriorityQueue<Tile>(width * height);
            Path = new List<int2>(width * height);
            Points = new List<Vector3>(width * height);
            Touched = new Dictionary<Tile, bool>(width * height);

            foreach (var navNode in navNodes)
            {
                navNode.Activated.AddListener(this.NavNodeActivated);
            }
        }

        private void NavNodeActivated(NavNode navNode)
        {
            if (this.PathStart == null)
            {
                this.PathStart = navNode;
            }
            else
            {
                var start = this.PathStart;
                this.PathStart = null;
                var end = navNode;
                var startCoords = new int2(start.X, start.Y);
                var endCoords = new int2(end.X, end.Y);
                var result = this.FindPath(startCoords, endCoords);
                if (result.IsNone)
                {
                    Debug.Log("No path found.");
                }
                else
                {
                    Debug.Log($"Found path consisting of {result.Points.Count} points with a cost of {result.Cost}.");
                }
            }
        }

        private Tile NavNodeToTile(NavNode navNode)
        {
            return this.Tiles[navNode.Y, navNode.X];
        }

        private NavNode TileToNavNode(Tile tile)
        {

            return this.Grid[tile.Y, tile.X];
        }

        private float Heuristic(Tile current, Tile goal)
        {
            return Mathf.Abs(current.X - goal.X) + Mathf.Abs(current.Y - goal.Y);
        }

        private float Weight(Tile current, Tile neighbor)
        {
            var isDiagonal = current.X != neighbor.X && current.Y != neighbor.Y;
            if (isDiagonal)
            {
                // Diagonal move costs more
                return 1.5f;
            }
            // Horizontal or vertical
            return 1f;
        }

        public PathResult FindPath(int2 startCoords, int2 goalCoords)
        {
            var touched = this.Touched;
            var openSet = OpenSet;
            var tiles = this.Tiles;
            var start = tiles[startCoords.y, startCoords.x];
            var goal = tiles[goalCoords.y, goalCoords.x];
            var path = this.Path;

            // Clean up previous state.
            foreach (var kv in touched)
            {
                kv.Key.Reset();
            }
            touched.Clear();

            // Clean up previous priority queue.
            openSet.Clear();

            // Clear previous path.
            path.Clear();

            #region Debug
            var h = this.Tiles.GetLength(0);
            var w = this.Tiles.GetLength(1);
            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    var tile = tiles[i, j];
                    if (!tile.IsReset())
                    {
                        Debug.Log($"Tile ({j},{i}) is not reset.");
                        tile.Reset();
                    }
                }
            }
            #endregion

            start.G = 0;
            start.F = Heuristic(start, goal);
            OpenSet.Enqueue(start, 0);
            touched[start] = true;

            Tile current;

            while (openSet.Count > 0)
            {
                current = openSet.Dequeue();

                if (current == goal)
                {
                    goto Reconstruct;
                }

                var neighbors = Neighbors[current.Y, current.X];
                var currentG = current.G;
                
                foreach (var neighbor in neighbors)
                {
                    var maybeG = currentG + Weight(current, neighbor);
                    if (maybeG < neighbor.G)
                    {
                        neighbor.G = maybeG;
                        neighbor.F = maybeG + Heuristic(neighbor, goal);
                        neighbor.Previous = current;
                        if (!openSet.Contains(neighbor))
                        {
                            touched[neighbor] = true;
                            openSet.Enqueue(neighbor, neighbor.F);
                        }
                    }
                }
            }

            // There is no path from start to goal.
            return PathResult.None;

        Reconstruct:
            var totalCost = 0.0f;
            while (current.Previous != null && current != start)
            {
                totalCost += current.G;
                path.Add(new int2(current.X, current.Y));
                current = current.Previous;
            }
            path.Add(new int2(start.X, start.Y));
            path.Reverse();

            if (this.DebugPath)
            {
                this.ShwoDebugPath(path);
            }

            var points = Points;
            points.Clear();
            foreach (var coords in path)
            {
                var navNode = this.Grid[coords.y, coords.x];
                var point = navNode.GetComponent<Renderer>().bounds.center;
                point.y = 0;
                points.Add(point);
            }

            StartCoroutine(PathMover.Move(this.Player, points, () => { }));

            return new PathResult { Points = path, Cost = totalCost };
        }

        public void OnSceneUnloaded()
        {
        }

        private void ShwoDebugPath(List<int2> path)
        {
            if (this.DebugPathObjects != null)
            {
                foreach (var gameObject in this.DebugPathObjects)
                {
                    GameObject.Destroy(gameObject);
                }
                this.DebugPathObjects = null;
            }
            this.DebugPathObjects = new List<GameObject>(path.Count);
            foreach (var coords in path)
            {
                var navNode = Grid[coords.y, coords.x];
                var pos = navNode.GetComponent<Renderer>().bounds.center;
                pos.y = 0;
                var gameObject = Instantiate(this.DebugSpherePrefab, pos, Quaternion.identity);
                this.DebugPathObjects.Add(gameObject);
            }
        }
    }
}