using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Enemies
{
    public class Goomba : Enemy
    {
        const float ACCEL = .1f;
        const int WIDTH = 50, HEIGHT = 50;        

        public override string TextureName => "enemies/goomba";
        public override string IconName => "Icons/goomba";
        internal override float WalkingSpeed => .75f;
        public override Point PreferredSize => new Point(WIDTH, HEIGHT);
        public override Point IconSize => new Point(100, 50);

        public enum Direction
        {
            Left,
            Right
        }
        Direction walkingDir;
        private bool Disabled;

        public Goomba(Point location, Direction WalkDirection = Direction.Left) : base(new Rectangle(location, new Point(WIDTH, HEIGHT)))
        {
            walkingDir = WalkDirection;
            PhysicsApplied = true;
            DefaultSource = false;
            OnCollision += Goomba_OnCollision;
        }

        private void Goomba_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            CollidedInto(type, collision, other);
        }

        public override void CollidedInto(Collidable.CollisionType type, Collidable col, GameObject other)
        {
            if (other is Character)
            {
                var character = other as Character;
                if (type == Collidable.CollisionType.FLOOR)
                {
                    character.Jump(8);
                    Die();
                }
                else
                    character.Harm();
            }
        }

        public override void Load()
        {
            base.Load();
            Animation = new Spritesheet(Texture, 1, 9);
        }

        public override void Update(GameTime gameTime)
        {
            VerifyMovement();
            animationTimer += gameTime.ElapsedGameTime;
            if (!InAir && !Disabled)
                Acceleration.X = ((walkingDir == Direction.Left) ? -1 : 1) * ACCEL;
            else if (!Disabled)
                Acceleration.X = 0;
            else
            {
                Acceleration.X = 0;
                Velocity.X = 0;
            }
            Animate();
            base.Update(gameTime);                    
        }

        void Animate()
        {
            if (animationTimer.TotalSeconds < .5f)
                return;
            animationTimer = TimeSpan.Zero;
            switch(Animation.Column)
            {
                case 0:
                    Source = Animation.AdvanceFrame();
                    break;
                case 1:
                    Source = Animation.AdvanceFrame(-1);
                    break;
            }
        }
    }
}
