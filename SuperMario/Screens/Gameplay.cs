using System;
using System.Collections.Generic;
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
        public Gameplay() : base(SCREENS.GAME)
        {

        }
        
        List<GameObject> GameObjects => Core.GameObjects;
        Camera GameCamera => Core.GameCamera;

        public override void Load(ContentManager content)
        {
            Core.levelData = LevelLoader.LevelData.LoadFile(@"D:\data.lev");
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
            var result = Core.ControlHandler.MenuKeys(Keyboard.GetState());
            if (result.Contains(Controls.MENUKeys.DEBUG_PLAYER_CREATE))
                GameObjects.Add(Player.DebugPlayer());
            if (result.Contains(Controls.MENUKeys.DEBUG_OBJECT_CREATE))
                GameObjects.Add(GameObject.CreateDebugObject());
            foreach (var obj in Core.SafeObjects)
            {
                obj.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (var obj in Core.SafeObjects)
                obj.Draw(sb);
        }
    }
}
