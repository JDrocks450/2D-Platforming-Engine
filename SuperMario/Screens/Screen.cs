using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperMario.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Screens
{
    public abstract class Screen
    {
        public virtual SpriteSortMode SortMode => SpriteSortMode.Deferred;

        public virtual Color Background => Color.Black;

        public bool Paused
        {
            get; set;
        }

        public enum SCREENS
        {
            LEVEL_START,
            GAME,
            CREATOR
        }

        public SCREENS UnderlyingType;

        public Screen(SCREENS type)
        {
            UnderlyingType = type;
        }

        internal List<GameObject> GameObjects => Core.GameObjects;
        internal Camera GameCamera => Core.GameCamera;

        public static Screen CreateScreen(SCREENS type)
        {
            switch (type)
            {
                case SCREENS.GAME:
                    return new Gameplay();
                case SCREENS.CREATOR:
                    return new LevelCreator();
                default:
                    return new Gameplay();
            }            
        }

        public abstract void Load(ContentManager content);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch batch);
    }
}
