using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.UI
{
    public class ObjectSpawner : UIComponent
    {
        const int MARGIN = 10;
        const int HEIGHT = 200;
        const int ITEM_SIZE = 50;

        public delegate void ObjectSpawnRequestHandler(byte id);
        public event ObjectSpawnRequestHandler OnObjectSpawnRequested;

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
            get; set;
        }
        public int Height
        {
            get; set;
        }
        public bool Disabled
        {
            get; set;
        }

        public bool Stretch, DropDown, AutoSize, Populated, ButtonsDisabled;

        /// <summary>
        /// Creates a ObjectSpawner for the Creator mode at the top of the screen
        /// </summary>
        public ObjectSpawner()
        {
            X = 0;
            Height = HEIGHT;
            Y = -Height;
            Width = Core.GameCamera.Screen.Width;
            Stretch = true;
            DropDown = true;
        }

        /// <summary>
        /// Creates the ObjectSpawner for use elsewhere
        /// </summary>
        /// <param name="viewBox"></param>
        public ObjectSpawner(Rectangle viewBox, bool autosize)
        {
            X = viewBox.X;
            Y = viewBox.Y;
            Width = viewBox.Width;
            Height = viewBox.Height;
            Stretch = false;
            AutoSize = autosize;
        }

        Dictionary<byte, Tuple<Texture2D, int, int>> items = new Dictionary<byte, Tuple<Texture2D, int, int>>();

        void Populate()
        {
            PopulateWith(typeof(LevelLoader.LevelData.OBJ_TABLE), 0);
        }

        /// <summary>
        /// Populates the ObjectSpawner with items
        /// </summary>
        /// <param name="source">Sets the source mode: 0 - OBJ_TABLE, 1 - ITEM_TABLE</param>
        /// <param name="exclude"></param>
        public void PopulateWith(Type Table, params byte[] exclude)
        {
            if (Populated)
                return;
            int source = 0;
            if (Table == typeof(LevelLoader.LevelData.OBJ_TABLE))
                source = 0;
            else if (Table == typeof(Items.Item.ITEM_TABLE))
                source = 1;
            int i = 0;
            foreach (byte obj in Enum.GetValues(Table))
            {
                if (exclude.Contains((byte)i))
                {
                    i++;
                    continue;
                }
                GameObject o = null;
                switch (source)
                {
                    case 0:
                        o = LevelLoader.LevelData.GetInstanceByID(obj, new Rectangle());
                        break;
                    case 1:
                        o = Items.Item.Parse(obj, out _);
                        break;
                }
                if (o == null)
                {
                    i++;
                    continue;
                }
                Texture2D texture = null;
                Point preferredSize = new Point(ITEM_SIZE);
                if (o is PrefabObjects.Prefab)
                {
                    texture = Core.Manager.Load<Texture2D>("Textures/" + (o as PrefabObjects.Prefab).IconName);
                    preferredSize = (o as PrefabObjects.Prefab).IconSize;
                }
                items.Add(obj, new Tuple<Texture2D, int, int>(texture, preferredSize.X, preferredSize.Y));
                i++;
            }
            Populated = true;
            if (AutoSize)
                doAutoSize();
        }

        void doAutoSize()
        {
            var width = MARGIN;
            var height = 0;
            foreach(var item in items)
            {
                width += item.Value.Item2 + MARGIN;
                if (item.Value.Item3 > height)
                    height = item.Value.Item3;
            }
            height += MARGIN;
            Width = width;
            Height = Height;
        }        

        int lastSCRHeight = 0;
        public void Update(GameTime gameTime)
        {          
            if(Stretch)
                Width = Core.GameCamera.Screen.Width;
            if (DropDown)
            {
                if (lastSCRHeight != Core.GameCamera.Screen.Height)
                {
                    Populate();
                    lastSCRHeight = Core.GameCamera.Screen.Height;
                }
                if (Y < 0)
                {
                    Y += 5;
                    return;
                }
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                ButtonsDisabled = false;
            var msr = new Rectangle(Mouse.GetState().Position, new Point(1));
            int i = 0;
            if (!ButtonsDisabled && itemBoxes != null)
            foreach (var r in itemBoxes) {
                if (msr.Intersects(r) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    OnObjectSpawnRequested?.Invoke(items.Skip(i).Take(1).First().Key);
                    ButtonsDisabled = true;
                    break;
                }
                i++;
            }
        }

        Rectangle[] itemBoxes;

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Core.BaseTexture, new Rectangle(X, Y, Width, Height), Color.DarkGray);
            int Xlast = X, Ylast = 0;
            bool tooltipOpened = false;
            itemBoxes = new Rectangle[items.Count];
            int i = 0;
            foreach(var tuple in items)
            {                
                int Xloc = Xlast + MARGIN;
                if (Xloc > X + Width)
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
                    Tooltip.ShowTooltip(this, Enum.GetName(typeof(LevelLoader.LevelData.OBJ_TABLE), tuple.Key));
                    tooltipOpened = true;
                }
                else if (!tooltipOpened)
                    Tooltip.HideTooltip(this);
                sb.Draw(tex,rect,color);
                itemBoxes[i] = rect;
                Xlast = Xloc + tuple.Value.Item2;
                i++;
            }
            Height = Ylast + ITEM_SIZE + MARGIN*2;
        }
    }
}
