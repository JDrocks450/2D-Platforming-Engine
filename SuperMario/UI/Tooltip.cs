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
    public class Tooltip
    {
        const int MARGIN = 5;
        static SpriteFont FONT;
        static int w, h;
        static Texture2D t;
        public static void Load(ContentManager m, int SCRWIDTH, int SCRHEIGHT, Texture2D texture)
        {
            FONT = m.Load<SpriteFont>("Fonts/Font");
            w = SCRWIDTH;
            h = SCRHEIGHT;
            t = texture;
        }

        public static bool Showing = true;
        public static string ShowingText = "EMPTY";
        public static object currentSender
        {
            get;
            private set;
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

        public static void Draw(SpriteBatch sb)
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
