using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    class Indestructible : Prefab
    {
        public override string TextureName => "unbreakable";
        public override Point PreferredSize => new Point(50);

        public Indestructible(Rectangle Box) : base(Box)
        {
            LimitedCollision = false;
        }
    }
}
