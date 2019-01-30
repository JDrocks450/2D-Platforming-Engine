using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class ObjectSpawner : UIComponent
    {
        const int MARGIN = 10;
        const int HEIGHT = 200;
        const int ITEM_SIZE = 50;

        public delegate void ObjectSpawnRequestHandler(byte id);
        public event ObjectSpawnRequestHandler OnObjectSpawnRequested;

        int X
        {
            get;set;
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

        public ObjectSpawner()
        {
            X = 0;
            Height = HEIGHT;
            Y = -Height;
            Width = Core.GameCamera.Screen.Width;            
        }

        Dictionary<byte, Tuple<Texture2D, int, int>> items = new Dictionary<byte, Tuple<Texture2D, int, int>>();

        void Populate()
        {
            if (Populated)
                return;
            foreach(byte obj in Enum.GetValues(typeof(LevelLoader.LevelData.OBJ_TABLE)))
            {
                var o = LevelLoader.LevelData.GetInstanceByID(obj, new Rectangle());
                Texture2D texture = null;
                Point preferredSize = new Point(ITEM_SIZE);
                if (o is PrefabObjects.Prefab)
                {
                    texture = Core.Manager.Load<Texture2D>("Textures/" + (o as PrefabObjects.Prefab).TextureName);
                    preferredSize = (o as PrefabObjects.Prefab).PreferredSize;
                }
                items.Add(obj, new Tuple<Texture2D, int, int>(texture, preferredSize.X, preferredSize.Y));
            }
            Populated = true;
        }

        bool Populated = false;
        int lastSCRHeight = 0;
        public override void Update()
        {
            Width = Core.GameCamera.Screen.Width;
            if (lastSCRHeight != Core.GameCamera.Screen.Height)
            {
                Populate();
                lastSCRHeight = Core.GameCamera.Screen.Height;
            }
            if (Y < 0)
            {
                Y += 1;
                return;
            }
            var msr = new Rectangle(Mouse.GetState().Position, new Point(1));
            int i = 0;
            foreach (var r in itemBoxes) {
                if (msr.Intersects(r) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    OnObjectSpawnRequested?.Invoke(items.Skip(i).Take(1).First().Key);
                    break;
                }
                i++;
            }
        }

        Rectangle[] itemBoxes;

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Core.BaseTexture, new Rectangle(X, Y, Width, Height), Color.DarkGray);
            int Xlast = 0, Ylast = 0;
            bool tooltipOpened = false;
            itemBoxes = new Rectangle[items.Count];
            int i = 0;
            foreach(var tuple in items)
            {                
                int Xloc = X + Xlast + MARGIN;
                if (Xloc > Width)
                {
                    Ylast += ITEM_SIZE;
                    Xlast = 0;
                }
                var tex = tuple.Value.Item1;
                if (tex == null)
                    tex = Core.BaseTexture;
                var color = Color.White;
                var rect = new Rectangle(Xloc, Y + Ylast + MARGIN, tuple.Value.Item2, tuple.Value.Item3);
                if (new Rectangle(Mouse.GetState().Position, new Point(1)).Intersects(rect))
                {
                    color = Color.White * .75f;
                    sb.Draw(Core.BaseTexture, rect, Color.White);
                    Tooltip.ShowTooltip(Enum.GetName(typeof(LevelLoader.LevelData.OBJ_TABLE), tuple.Key));
                    tooltipOpened = true;
                }
                else if (!tooltipOpened)
                    Tooltip.HideTooltip();
                sb.Draw(tex,rect,color);
                itemBoxes[i] = rect;
                Xlast = Xloc + tuple.Value.Item2;
                i++;
            }
            Height = Ylast + ITEM_SIZE + MARGIN*2;
        }
    }
}
