using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Assets.Scripts
{
    public class PathResult
    {
        public static PathResult None { get; } = new PathResult { Points = new List<int2>(0), Cost = float.PositiveInfinity };

        public List<int2> Points;

        public float Cost;

        public bool IsNone => this.Points.Count == 0;
    }
}
