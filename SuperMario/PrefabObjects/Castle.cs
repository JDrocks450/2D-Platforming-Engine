using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class Castle : Prefab
    {
        public override string TextureName => "castle";
        public override Point PreferredSize => new Point(WIDTH, HEIGHT);
        public override Point IconSize => new Point(50,50);

        public const int WIDTH = 200, HEIGHT = 200;

        public Castle(Point Location) : base(new Rectangle(Location, new Point(WIDTH, HEIGHT)))
        {
            CollisionGhosted = true;
            CalculateCollision = false;
        }
    }
}
