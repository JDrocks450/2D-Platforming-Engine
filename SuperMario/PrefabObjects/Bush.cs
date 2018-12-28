﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class Bush : Prefab
    {
        public override string TextureName => "bush";

        public const int WIDTH = 250, HEIGHT = 50;

        public Bush(Point Location) : base(new Rectangle(Location, new Point(WIDTH, HEIGHT)))
        {
            Physics_Ghost = true;
        }
    }
}
