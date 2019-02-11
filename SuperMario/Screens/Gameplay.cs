using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.ControlMapper;
using SuperMario.Enemies;
using SuperMario.Items;
using SuperMario.PrefabObjects;

namespace SuperMario.Screens
{
    public class Gameplay : Screen
    {
        public override Color Background => Color.SkyBlue;

        public override SpriteSortMode SortMode => SpriteSortMode.FrontToBack;

        public Gameplay() : base(SCREENS.GAME)
        {

        }

        bool showLevStart = true;

        void ShowLevelStart()
        {
            var s = new Level_Start(this, "world 1 stage 1", Core.Lives);
            s.Load(Core.Manager);
            Core.UIElements.Add(s);
            Paused = true;
            showLevStart = false;
        }

        public override void Load(ContentManager content)
        {                        
            if (Core.levelData.LoadedObjects.Count == 0)
            {
                GameObjects.Add(new Ground(new Rectangle(0, 350, 1000, 0)));
                GameObjects.Add(new Block(new Rectangle(300, 150, 50, 50)));
                GameObjects.Add(new QuestionBlock(new Rectangle(350, 150, 50, 50), QuestionBlock.SpawnObjectLogic.Inferred));
                GameObjects.Add(new QuestionBlock(new Rectangle(400, 150, 50, 50), QuestionBlock.SpawnObjectLogic.Inferred));
                GameObjects.Add(new Block(new Rectangle(450, 150, 50, 50)));
                GameObjects.Add(new Goomba(new Point(500, 0)));
            }
            else
                GameObjects.AddRange(Core.levelData.LoadedObjects);
        }

        public override void Update(GameTime gameTime)
        {            
            GameCamera.Focus = Core.ControlledPlayer;
            Core.SafeObjects = GameObjects.ToArray();
            if (Paused)
                return;
            var result = Core.ControlHandler.MenuKeys(Keyboard.GetState());
            if (result.Contains(Controls.MENUKeys.DEBUG_PLAYER_CREATE))
                GameObjects.Add(Player.DebugPlayer());
            if (result.Contains(Controls.MENUKeys.DEBUG_OBJECT_CREATE))
                GameObjects.Add(GameObject.CreateDebugObject());
            if (result.Contains(Controls.MENUKeys.DEBUG_GOOMBA_CREATE))
                GameObjects.Add(new Goomba(new Point((int)(Core.ControlledPlayer?.Location ?? new Vector2()).X, 0), Goomba.Direction.Right));
            foreach (var obj in Core.SafeObjects)
            {
                obj.Update(gameTime);
            }            
            Collidable.Final();
            if (!GameObjects.OfType<Player>().Any() && !showLevStart)
                showLevStart = true;
            if (showLevStart)
            {
                ShowLevelStart();
                if (!GameObjects.OfType<Player>().Any())
                    GameObjects.Add(Player.DebugPlayer());
            }
        }

        public override void OnExiting()
        {
            
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (var obj in Core.SafeObjects)
                obj.Draw(sb);
        }
    }
}
