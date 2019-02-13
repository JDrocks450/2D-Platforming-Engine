using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMario.Enemies
{
    public class Fireball : Character, Character.Enemy
    {
        public override string TextureName => "Enemies/fireball";        
        public override Point PreferredSize => new Point(35);
        const float SPEED = 8;
        Vector2 Speed
        {
            get => new Vector2((Facing == Goomba.Direction.Left ? -1 : 1) * SPEED, SPEED);
        }

        public Fireball(Point origin, Direction d) : base(new Rectangle(origin, new Point()))
        {
            OnCollision += Fireball_OnCollision;
            GravityApplied = false;
            Facing = d;
            Velocity = Speed;            
        }

        int facingTimer = -1;
        private void Fireball_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            CollidedInto(type, collision, other);
        }

        public override void CollidedInto(Collidable.CollisionType type, Collidable col, GameObject other)
        {
            if (other is Enemy && !(other is Fireball))
            {
                (other as Character).Harm();
                Remove();
            }
            else if (type == Collidable.CollisionType.WALL)
            {
                if (facingTimer > 0)
                    return;
                Facing = Facing == Direction.Right ? Direction.Left : Direction.Right;
                facingTimer = 0;
                bounces++;
            }
        }

        int bounces = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (facingTimer >= 0)
                if (facingTimer < 5)
                    facingTimer++;
                else
                    facingTimer = -1;
            Velocity.X = Speed.X;
            if (!InAir)
            {
                GravityApplied = true;
                Jump(8f);                
            }
            if (bounces > 5)
                Remove();
        }

        int animTimer = 0;
        public override void Draw(SpriteBatch sb)
        {
            effects = Facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            base.Draw(sb);
            if (animTimer == 3)
            {
                Graphics_Rotation += Facing == Direction.Right ? 1 : -1 * MathHelper.ToRadians(45);                
                animTimer = 0;
            }
            animTimer++;
        }
    }
}
