using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class BigCastle : Prefab
    {
        public override string TextureName => "bigcastle";
        public override Point PreferredSize => new Point(WIDTH, HEIGHT);
        public override Point IconSize => new Point(50, 50);

        public const int WIDTH = 350, HEIGHT = 400;

        public BigCastle(Point Location) : base(new Rectangle(Location, new Point(WIDTH, HEIGHT)))
        {
            CollisionGhosted = true;
            CalculateCollision = false;
        }
    }
}
