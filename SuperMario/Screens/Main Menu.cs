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
    public class Main_Menu : Screen, UI.UIComponent
    {
        Texture2D logo;

        public override Color Background => Color.Black;

        public int X { get => 0; set { } }
        public int Y { get => 0; set { } }
        public int Width { get; set; } = Core.SCRWIDTH;
        public int Height { get; set; } = Core.SCRHEIGHT;
        public bool Disabled { get; set; } = false;

        public Main_Menu() : base(SCREENS.LEVEL_START)
        {

        }

        public override void Load(ContentManager content)
        {
            logo = content.Load<Texture2D>("Textures/logo");
            if (Core.CurrentScreen != null)
                Core.CurrentScreen.Paused = true;
        }

        public override void OnExiting()
        {
            
        }

        TimeSpan _shown;
        float animHeight = 0;
        int selection = 0;

        void UI.UIComponent.Update(GameTime gameTime)
        {
            var r = Core.ControlHandler.GetKeyControl();
            if (r.Contains("up"))
                selection--;
            else if (r.Contains("down"))
                selection++;            
            if (selection < 0)
                selection = 0;
            if (selection > 1)
                selection = 1;
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                SCREENS scr = SCREENS.GAME;
                switch (selection)
                {
                    case 1:
                        scr = SCREENS.CREATOR;
                        break;
                }
                if (Core.CurrentScreen != null)
                    Core.CurrentScreen.OnExiting();
                Core.GameCamera = new Camera();
                Core.CurrentScreen = CreateScreen(scr);
                Core.PostMainMenu();               
                Core.UIElements.Remove(this);
                Core.CurrentScreen.Paused = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            (this as UI.UIComponent).Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            int lW = 500, lH = 250;
            var logoRect = new Rectangle((Width / 2) - lW / 2, (Height / 2) - lH / 2, lW, lH);
            batch.Draw(Core.BaseTexture, new Rectangle(0, 0, Width, Height), Color.Gray);
            batch.Draw(logo, logoRect, Color.White);                
            var item1 = "PLAY";
            var size = Core.Font.MeasureString(item1); 
            batch.DrawString(Core.Font, item1, new Vector2((Width / 2) - (size.X/2), 
                (logoRect.Y + lH) - (size.Y/2) + 20), selection == 0 ? Color.White : Color.Black);
            var item2 = "CREATE";
            var size2 = Core.Font.MeasureString(item2);
            batch.DrawString(Core.Font, item2, new Vector2((Width / 2) - (size.X / 2),
                ((logoRect.Y + lH) - (size.Y / 2)) + size2.Y + 40), selection == 1 ? Color.White : Color.Black);
        }
    }
}
