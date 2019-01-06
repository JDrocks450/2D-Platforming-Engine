using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Screens
{
    public abstract class Screen
    {
        public enum SCREENS
        {
            LEVEL_START,
            GAME
        }

        public SCREENS UnderlyingType;

        public Screen(SCREENS type)
        {
            UnderlyingType = type;
        }

        public static Screen CreateScreen(SCREENS type)
        {
            switch (type)
            {
                case SCREENS.GAME:
                    return new Gameplay();
                default:
                    return new Gameplay();
            }            
        }

        public abstract void Load(ContentManager content);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch batch);
    }
}
