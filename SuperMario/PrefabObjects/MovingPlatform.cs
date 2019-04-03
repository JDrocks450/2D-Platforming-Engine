using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class MovingPlatform : Prefab
    {
        public Vector2 Low, High;
        public override string TextureName => "";

        public MovingPlatform(Vector2 low, Vector2 high, Rectangle start) : base(start)
        {
            Low = low;
            High = high;
        }
    }
}
