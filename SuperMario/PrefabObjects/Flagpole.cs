using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class Flagpole : Prefab
    {
        const int FLAG_MAX_HEIGHT = 25;
        const int CUTSCENE_LENGTH = 60;

        public override string IconName => "rflag";
        public override Point PreferredSize => new Point(100, 5*50);
        public override Point IconSize => new Point(75, 50);        

        public Flagpole(Point Location) : base(new Rectangle(Location, new Point(0)), true)
        {
            DefaultSource = false;
            ZIndex = .9f;
            TextureColor = Color.Gray;
        }

        Texture2D[] texts = new Texture2D[3];
        public override void Load()
        {
            texts[0] = Core.Manager.Load<Texture2D>("Textures/ftop");
            texts[1] = Core.Manager.Load<Texture2D>("Textures/rflag");
            texts[2] = Core.Manager.Load<Texture2D>("Textures/flagpole");
            Source = new Rectangle();
        }

        float _flagYOffset = FLAG_MAX_HEIGHT;
        float _playerSetFlagHeight = -1;
        int _height;
        int _timer = 0;
        bool RunningCutscene = false;
        GameObject standing;

        Rectangle getVisibleHitbox
        {
            get => new Rectangle(Location.ToPoint(), new Point(Width, _height));
        }

        public override void Update(GameTime gameTime)
        {            
            _height = standing is null ? Core.WORLD_BOTTOM : (int)standing.Y - (int)Y;
            ManualCameraFollowPoint = true;
            CameraFollowPoint = new Vector2(25, _height / 2);
            Height = 100; //For level creator
            Width = 100; //For Level creator
            if (standing == null)
                standing = Core.SafeObjects.Where(x => !(x is Flagpole) && x.Hitbox.Intersects(getVisibleHitbox))?.First();
            Source.Size = Hitbox.Size;
            if (Core.ControlledPlayer != null)
                if (Core.ControlledPlayer.Hitbox.Intersects(getVisibleHitbox))
                    RunningCutscene = true;
            if (RunningCutscene)
                RunCutscene();
        }

        void RunCutscene()
        {
            if (ProgramedCutscene1())
                ProgrammedCutscene2();
        }

        void ProgrammedCutscene2()
        {
            var p = Core.ControlledPlayer;
            if (_timer == CUTSCENE_LENGTH)
            {
                TextureColor = Color.Red;
                p.Velocity.Y = ((float)_height - FLAG_MAX_HEIGHT - p.Height) / CUTSCENE_LENGTH;
                _playerSetFlagHeight = p.Y - Y;
            }
            if (_timer < CUTSCENE_LENGTH * 2)
            {
                p.X = (X - p.Width) + 30;
                _flagYOffset -= (_height - _playerSetFlagHeight) / CUTSCENE_LENGTH;
                _timer++;
                return;
            }            
            if (_timer < CUTSCENE_LENGTH*2 + 10)
            {
                p.X = X + 30;
                _timer++;
                return;
            }
            p.GravityApplied = true;
            p.Velocity.X = 10;
            p.Jump(10);
            RunningCutscene = false;
        }

        bool ProgramedCutscene1()
        {
            if (_timer > 60)
                return true;
            var p = Core.ControlledPlayer;
            Core.GameCamera.Focus = this;
            p.GravityApplied = false;
            p.Velocity = Vector2.Zero;
            p.X = (X - p.Width) + 30;            
            if (_timer < 60)
            {
                p.Acceleration = new Vector2(0);
                _flagYOffset += ((float)_height - FLAG_MAX_HEIGHT - 40) / 60;
                _timer++;
                return false;
            }
            return true;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texts[0], new Rectangle(Location.ToPoint(), new Point(50)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, ZIndex);
            for(int i = 0; i < (_height - 50) / 50; i++)            
                sb.Draw(texts[2], new Rectangle((int)X, (int)Y + 50 + (50 * i), 50, 50), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, ZIndex);
            sb.Draw(texts[1], new Rectangle((int)(X + 31), (int)(Y + _flagYOffset), 75, 40), null, TextureColor, 0f, Vector2.Zero, SpriteEffects.None, ZIndex);
        }
    }
}
