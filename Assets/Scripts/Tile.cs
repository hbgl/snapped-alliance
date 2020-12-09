using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Tile : FastPriorityQueueNode
    {
        public int X { get; }

        public int Y { get; }

        public float G;

        public float F;

        public Tile Previous;

        public Tile(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Reset();
        }

        public void Reset()
        {
            this.ResetScore();
            this.Previous = null;
        }

        public void ResetScore()
        {
            this.G = float.PositiveInfinity;
            this.F = float.PositiveInfinity;
        }

        public bool IsReset()
        {
            return float.IsInfinity(this.F) && float.IsInfinity(this.G) && this.Previous == null;
        }
    }
}
