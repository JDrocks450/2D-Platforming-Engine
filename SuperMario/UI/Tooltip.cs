using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.UI
{
    public abstract class Tooltip : UIComponent
    {
        const int MARGIN = 5;
        public static SpriteFont FONT;
        static int w, h;

        public int X
        {
            get; set;
        }
        public int Y
        {
            get; set;
        }
        public int Width
        {
            get => w; set => w = value;
        }
        public int Height
        {
            get => h; set => h = value;
        }

        public static bool Showing
        {
            get; set;
        } = false;

        public static string ShowingText
        {
            get; set;
        } = "EMPTY";

        public static object currentSender
        {
            get;
            private set;
        }

        static Texture2D t;
        public static void Load(ContentManager m, int SCRWIDTH, int SCRHEIGHT, Texture2D texture)
        {
            FONT = m.Load<SpriteFont>("Fonts/Tooltip");
            w = SCRWIDTH;
            h = SCRHEIGHT;
            t = texture;
        }
        
        public static void ShowTooltip(object sender, string TEXT)
        {
            ShowingText = TEXT;
            Showing = true;
            currentSender = sender;
        }

        public static void HideTooltip(object sender)
        {
            if (currentSender == sender)
                Showing = false;
        }

        public abstract void Update(GameTime gt);
        public abstract void Draw(SpriteBatch sb);

        public static void sDraw(SpriteBatch sb)
        {
            if (!Showing)
                return;
            Point p = Mouse.GetState().Position;
            var tSize = FONT.MeasureString(ShowingText);
            var rect = new Rectangle(p.X + MARGIN, p.Y + MARGIN, 2 * MARGIN + (int)tSize.X, 2 * MARGIN + (int)tSize.Y);
            var rect2 = new Rectangle(p.X+1 + MARGIN, p.Y+1 + MARGIN, (2 * MARGIN + (int)tSize.X) - 2, (2 * MARGIN + (int)tSize.Y) - 2);
            if (rect.Right > w)
            {
                rect.X -= (rect.Width + MARGIN);
                rect2.X = rect.X + 1;
            }            
            sb.Draw(t, rect, Color.Black);
            sb.Draw(t, rect2, Color.White);
            sb.DrawString(FONT, ShowingText, (rect.Location + new Point(MARGIN)).ToVector2(), Color.Black);
        }
    }
}
