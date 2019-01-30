using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.UI
{
    public abstract class UIComponent
    {
        int X
        {
            get; set;
        }

        int Y
        {
            get; set;
        }

        int Width
        {
            get; set;
        }

        int Height
        {
            get; set;
        }

        public abstract void Update();

        public abstract void Draw(SpriteBatch sb);
    }
}
