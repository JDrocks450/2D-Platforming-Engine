using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Components
{
    public class Effect_Tile
    {
        /// <summary>
        /// 0: Left Corner
        /// 1: Border Middle
        /// 2: Right Corner
        /// 3: Middle
        /// </summary>
        public virtual string[] TileTextureNames
        {
            get;
        }

        public Point TileSize
        {
            get;
        }

        public Rectangle FillArea
        {
            get;
            set;
        }

        public void Tile(SpriteBatch sb)
        {
            int rows = (int)((float)FillArea.Height / TileSize.Y);
        }
    }
}
