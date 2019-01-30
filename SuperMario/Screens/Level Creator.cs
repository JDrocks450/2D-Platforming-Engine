using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.LevelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Screens
{
    public class LevelCreator : Screen
    {
        class CameraObject : GameObject
        {
            public CameraObject() : base(null, new Rectangle(0, 0, 1, 1))
            {

            }

            const int CAM_SPEED = 10;
            public override void Update(GameTime gameTime)
            {
                var result = Core.ControlHandler.GetKeyControl(Keyboard.GetState());
                if (result.Contains("left"))
                    X -= CAM_SPEED;
                if (result.Contains("right"))
                    X += CAM_SPEED;
                if (result.Contains("up"))
                    Y -= CAM_SPEED;
                if (result.Contains("down"))
                    Y += CAM_SPEED;
            }

            public override void Draw(SpriteBatch sb)
            {
                
            }
        }

        const int GRID_SIZE = 50;

        CameraObject co;
        ObjectSpawner os;
        public LevelCreator() : base(SCREENS.CREATOR)
        {
            Core.DEBUG = false;
            Core.MouseVisible = true;
            co = new CameraObject();
            Core.GameCamera.CameraIgnoreWorldBottom = true;
            os = new ObjectSpawner();
            os.OnObjectSpawnRequested += Os_OnObjectSpawnRequested;
        }

        private void Os_OnObjectSpawnRequested(byte id)
        {
            GameObjects.Add(LevelLoader.LevelData.GetInstanceByID(id, new Rectangle(0,0,0,0)));
        }

        public void LoadLevel(LevelData source)
        {
            Core.levelData = source;
        }
        
        public override void Load(ContentManager content)
        {
            GameObjects.AddRange(Core.levelData.LoadedObjects);
        }

        Point mouseLastPos;
        public override void Update(GameTime gameTime)
        {
            if (!Core.UIElements.Contains(os))
                Core.UIElements.Add(os);
            Core.GameCamera.Focus = co;
            Core.SafeObjects = GameObjects.ToArray();
            co.Update(gameTime);
            foreach (var obj in Core.SafeObjects)
            {
                obj.PhysicsApplied = false;
                if (isDragging)
                {
                    if (dragging == obj)
                    {
                        obj.Location = Snap();
                        UI.Tooltip.ShowTooltip("Moving Object... Press DEL to Delete.");
                    }
                }
                else
                {
                    UI.Tooltip.HideTooltip();
                }
                if (dragging != obj)
                    obj.Update(gameTime);
                CheckObjectMouseCollision(obj);
            }
            mouseLastPos = Mouse.GetState().Position;
        }

        GameObject dragging;
        bool isDragging = false;

        Vector2 Snap()
        {
            var loc = Core.MousePosition;
            int X = loc.X / GRID_SIZE;
            int Y = loc.Y / GRID_SIZE;
            return new Vector2(GRID_SIZE * X, GRID_SIZE * Y);
        }

        void DrawGrid(SpriteBatch sb)
        {
            int start = (Core.GameCamera.Screen.X / 50) * 50;
            for (int c = 0; c < Core.WORLD_BOTTOM; c += GRID_SIZE)
            {
                for (int i = start < 0 ? 0 : start; i < GameCamera.Screen.Right; i += GRID_SIZE)
                {
                    sb.Draw(Core.BaseTexture, new Rectangle(i, c, 2, GRID_SIZE), Color.White);
                }
                sb.Draw(Core.BaseTexture, 
                    new Rectangle(GameCamera.Screen.X < 0 ? 0 : GameCamera.Screen.X, c, GameCamera.Screen.Width, 2), Color.White);
            }
        }

        void CheckObjectMouseCollision(GameObject obj)
        {
            var mouse = Mouse.GetState();
            var mouseRect = new Rectangle(Core.MousePosition.X, Core.MousePosition.Y, 1, 1);
            if (mouseRect.Intersects(obj.Hitbox))
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    startDrag(obj);
                }
            if (isDragging)
                if (mouse.LeftButton == ButtonState.Released)
                {
                    isDragging = false;
                    dragging = null;
                }
        }

        void startDrag(GameObject obj)
        {
            if (!isDragging)
            {
                dragging = obj;
                isDragging = true;
            }
        }

        public override void Draw(SpriteBatch batch)
        {            
            foreach (var obj in Core.SafeObjects)
                obj.Draw(batch);
            batch.Draw(Core.BaseTexture, new Rectangle(Core.MousePosition.X, Core.MousePosition.Y, 25, 25), Color.Orange * .5f);
            DrawGrid(batch);
        }
    }
}
