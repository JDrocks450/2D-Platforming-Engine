using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Items
{
    public class Mushroom : Items.Item
    {
        const int MOVE_SPEED = 2;

        public override string TextureName => "mushroom";

        public Mushroom() : base(new Microsoft.Xna.Framework.Rectangle(0, 0, 50, 50))
        {

        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            if (Velocity.X == 0 && !InAir)
                Velocity.X = MOVE_SPEED;
            base.Update(gameTime);
        }

        public override void Interact(Player other)
        {
            other.ChangePowerupState(Player.PowerupState.REG);
            Remove();
        }
    }
}
