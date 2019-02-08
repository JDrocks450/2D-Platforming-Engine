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

        public Fireball(Point origin) : base(new Rectangle(origin, new Point()))
        {
            OnCollision += Fireball_OnCollision;
            GravityApplied = false;
            Velocity = new Vector2(SPEED);
        }

        private void Fireball_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            if (other is Enemy)
                (other as Character).Harm();
            Remove();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!InAir)
            {
                GravityApplied = true;
                Jump(8f);
                Velocity.X = SPEED;
            }
        }

        int animTimer = 0;
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (animTimer == 5)
            {
                Graphics_Rotation += MathHelper.ToRadians(45);
                animTimer = 0;
            }
            animTimer++;
        }
    }
}
