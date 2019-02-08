using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMario.Screens
{
    public class Level_Start : Screen, UI.UIComponent
    {
        const float SECONDS_SHOWN = 2;

        Screen parent;
        Texture2D playerIcon;
        string levelHeader;
        int lives = 5;

        public int X { get => 0; set { } }
        public int Y { get => 0; set { } }
        public int Width { get; set; } = Core.SCRWIDTH;
        public int Height { get; set; } = Core.SCRHEIGHT;

        public Level_Start(Screen FocusTo, string World, int Lives) : base(SCREENS.LEVEL_START)
        {
            parent = FocusTo;
            levelHeader = World;
            lives = Lives;
        }

        public override void Load(ContentManager content)
        {
            playerIcon = content.Load<Texture2D>("Textures/Icons/mario");   
        }

        TimeSpan _shown;
        float animHeight = 0;

        void UI.UIComponent.Update(GameTime gameTime)
        {
            _shown += gameTime.ElapsedGameTime;
            if (_shown.TotalSeconds < SECONDS_SHOWN - 1)
            {
                if (animHeight < Height)
                    animHeight += Height / 10;
            }
            else
                animHeight -= Height / 10;
            if (_shown.TotalSeconds > SECONDS_SHOWN)
            {
                Core.UIElements.Remove(this);
                parent.Paused = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            (this as UI.UIComponent).Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(Core.BaseTexture, new Rectangle(0, 0, Width, (int)animHeight), Background);
            var size = Core.Font.MeasureString(levelHeader);
            batch.DrawString(Core.Font, levelHeader, new Vector2((Width / 2) - (size.X/2), (Height / 2) - (size.Y/2)), Color.White);
            batch.Draw(playerIcon, 
                new Rectangle((Width / 2) - playerIcon.Width - 20, (Height/2) + 20, playerIcon.Width, playerIcon.Height),
                Color.White);
            var x = "x";
            size = Core.Font.MeasureString(x);
            batch.DrawString(Core.Font, x, new Vector2((Width / 2) - (size.X / 2), (Height / 2) + 20 + (playerIcon.Height / 2)), Color.White);
            batch.DrawString(Core.Font, lives.ToString(), new Vector2((Width / 2) + size.X + 5, (Height / 2) + 20 + (playerIcon.Height / 2)), Color.White);
        }
    }
}
