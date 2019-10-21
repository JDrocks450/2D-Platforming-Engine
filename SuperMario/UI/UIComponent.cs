using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.UI
{
    public interface UIComponent
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

        bool Disabled
        {
            get; set;
        }

        void Update(GameTime gt);

        void Draw(SpriteBatch sb);
    }
}
