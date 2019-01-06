using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SuperMario.Items
{
    public class FireFlower : Item
    {
        public override string TextureName => "fireflower";

        public FireFlower() : base(new Microsoft.Xna.Framework.Rectangle(0, 0, 50, 50))
        {

        }

        public override void Interact(Player other)
        {
            other.ChangePowerupState(Player.PowerupState.FIRE);
        }
    }
}
