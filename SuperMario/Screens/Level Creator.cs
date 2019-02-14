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
                var result = Core.ControlHandler.GetKeyControl();
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
        public override Color Background => Color.SkyBlue;        

        enum PLACEMENT_MODE
        {
            NONE,
            DRAG,
            RESIZE,
            DELETE,
            ERASER,
            MULTI_PLACE
        }

        PLACEMENT_MODE currentPMode = PLACEMENT_MODE.NONE;

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
            GameObjects.Add(LevelLoader.LevelData.GetInstanceByID(id, new Rectangle(Snap(co.Location.ToPoint()).ToPoint(), new Point(0))));
        }

        public override void OnExiting()
        {
            Core.levelData.WriteAllObjects(GameObjects);
        }

        public void LoadLevel(LevelData source)
        {
            Core.levelData = source;
        }
        
        public override void Load(ContentManager content)
        {
            GameObjects.AddRange(Core.levelData.LoadedObjects);
            foreach (var obj in GameObjects)
                CorrectObjectPosition(obj);
        }        

        bool CheckIfInsideObject(GameObject obj, out GameObject intersect)
        {
            return CheckIfInsideObject(obj.Location, obj.Width, obj.Height, out intersect, obj);            
        }
        bool CheckIfInsideObject(Vector2 p, int w, int h, out GameObject intersects, GameObject exclude = null)
        {
            foreach (var intersect in GameObjects)
            {
                if (intersect.Hitbox.Intersects(new Rectangle(p.ToPoint(), new Point(w, h))) && intersect != exclude)
                {
                    intersects = intersect;
                    return true;
                }
            }
            intersects = null;
            return false;
        }

        void GetInputs()
        {
            var r = Keyboard.GetState().GetPressedKeys();
            if (!r.Any() && currentPMode != PLACEMENT_MODE.NONE)
            {
                currentPMode = PLACEMENT_MODE.DRAG;
                return;
            }
            foreach (var key in r)
                switch (key)
                {
                    case Keys.LeftControl:
                        if (currentPMode != PLACEMENT_MODE.NONE)
                        {
                            currentPMode = PLACEMENT_MODE.RESIZE;
                        }
                        break;
                    case Keys.Delete:
                        if (currentPMode != PLACEMENT_MODE.NONE && currentPMode != PLACEMENT_MODE.ERASER)
                        {
                            currentPMode = PLACEMENT_MODE.DELETE;
                        }
                        else if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            currentPMode = PLACEMENT_MODE.ERASER;
                        }
                        break;
                    case Keys.LeftShift:
                        if (currentPMode != PLACEMENT_MODE.NONE)
                        {
                            currentPMode = PLACEMENT_MODE.MULTI_PLACE;
                        }
                        break;
                }
        }

        int infiplaceCount = 0;
        Vector2 lastInfiniPlaceSuccess = Vector2.Zero;

        void DoInfiPlace()
        {
            UI.Tooltip.ShowTooltip(this, "Painting Object... " + infiplaceCount + " objects placed");
            var obj = Holding;
            var pos = Snap();
            if (lastInfiniPlaceSuccess != pos)
            {
                if (!CheckIfInsideObject(pos, obj.Width, obj.Height, out _))
                {
                    var ID = LevelData.GetIDByInstance(obj);
                    var newObj = LevelData.GetInstanceByID(ID, new Rectangle((int)pos.X, (int)pos.Y, 0, 0));
                    GameObjects.Add(newObj);
                    infiplaceCount++;
                    lastInfiniPlaceSuccess = pos;
                }
                else
                    UI.Tooltip.ShowTooltip(this, "Can't Place Object Here...");
            }
        }

        void DoResize(GameObject obj)
        {
            UI.Tooltip.ShowTooltip(this, "Resizing Objects not implemented!");
        }

        void DoDrag(GameObject obj)
        {
            UI.Tooltip.ShowTooltip(this, "Moving Object... Press DEL to Delete.");
            if (isObjectHeld)
            {
                if (Holding == obj)
                {
                    var pos = Snap();
                    if (!CheckIfInsideObject(pos, obj.Width, obj.Height, out GameObject i, obj))
                    {
                        obj.Location = pos;
                        CorrectObjectPosition(obj);                        
                    }
                    else
                        UI.Tooltip.ShowTooltip(this, "Can't Move Object Here.");
                }
            }
        }

        void DoDelete(GameObject obj)
        {
            if (isObjectHeld)
                if (obj == Holding)
                {
                    GameObjects.Remove(obj);
                    dropObject();
                }
        }

        private void DoEraser()
        {           
            var pos = Snap();
            UI.Tooltip.ShowTooltip(this, "Erasing Objects... " + infiplaceCount + " deleted.");
            if (CheckIfInsideObject(pos, GRID_SIZE, GRID_SIZE, out GameObject i))
            {
                GameObjects.Remove(i);
                infiplaceCount++;
            }
        }

        void RunObjectPlacementMode(GameObject focus)
        {
            switch (currentPMode)
            {
                case PLACEMENT_MODE.DRAG:
                    DoDrag(focus);
                    break;
                case PLACEMENT_MODE.RESIZE:
                    DoResize(focus);
                    break;
                case PLACEMENT_MODE.DELETE:
                    DoDelete(focus);
                    break;
                case PLACEMENT_MODE.MULTI_PLACE:
                    DoInfiPlace();
                    break;
                case PLACEMENT_MODE.ERASER:
                    DoEraser();
                    break;
            }
        }        

        Point mouseLastPos;
        public override void Update(GameTime gameTime)
        {
            if (!Core.UIElements.Contains(os))
                Core.UIElements.Add(os);
            Core.GameCamera.Focus = co;
            Core.SafeObjects = GameObjects.ToArray();
            if (Paused)
                return;
            co.Update(gameTime);
            GetInputs();
            RunObjectPlacementMode(Holding);
            foreach (var obj in Core.SafeObjects)
            {
                obj.CalculateCollision = false;
                obj.Disabled = true;
                if (currentPMode == PLACEMENT_MODE.NONE)
                {
                    UI.Tooltip.HideTooltip(this);
                }
                if (Holding != obj)
                    obj.Update(gameTime);
                CheckObjectMouseCollision(obj);
            }
            mouseLastPos = Mouse.GetState().Position;
        }

        GameObject Holding;
        bool isObjectHeld = false;

        Vector2 Snap(Point Loc = default)
        {
            if(Loc == default)
                Loc = Core.MousePosition;
            int X = Loc.X / GRID_SIZE;
            int Y = Loc.Y / GRID_SIZE;
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

        public void CorrectObjectPosition(GameObject obj)
        {
            if (obj.X < 0)
                obj.X = 0;
            if (obj.Y < 0)
                obj.Y = 0;
            if (obj.Y > Core.WORLD_BOTTOM)
            {
                obj.Y = Core.WORLD_BOTTOM;
            }
        }

        Rectangle getVisibleHitbox(GameObject obj)
        {
            return new Rectangle(obj.Location.ToPoint(), obj.Source.Size);
        }

        void CheckObjectMouseCollision(GameObject obj)
        {
            var mouse = Mouse.GetState();
            var mouseRect = new Rectangle(Core.MousePosition.X, Core.MousePosition.Y, 1, 1);
            if (currentPMode == PLACEMENT_MODE.NONE)
                if (mouseRect.Intersects(getVisibleHitbox(obj)))
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        startDrag(obj);
                    }
            if (mouse.LeftButton == ButtonState.Released)
            {
                dropObject();
            }
        }

        void dropObject()
        {
            isObjectHeld = false;
            Holding = null;
            currentPMode = PLACEMENT_MODE.NONE;
            lastInfiniPlaceSuccess = Vector2.Zero;
            infiplaceCount = 0;
        }

        void startDrag(GameObject obj)
        {
            if (!isObjectHeld)
            {
                Holding = obj;
                isObjectHeld = true;
                currentPMode = PLACEMENT_MODE.DRAG;
            }
        }

        public override void Draw(SpriteBatch batch)
        {            
            foreach (var obj in Core.SafeObjects)
                obj.Draw(batch);
            DrawGrid(batch);
        }
    }
}
