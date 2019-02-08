using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class Block : Prefab
    {
        public override string TextureName => "brick";
        public override Point PreferredSize => new Point(50);

        public Block(Rectangle Box) : base(Box)
        {
            LimitedCollision = false;
            OnCollision += Block_OnCollision;
        }

        private void Block_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            if (other == Core.ControlledPlayer && type == Collidable.CollisionType.CEILING) //Player hits bottom  
                Interact(((Player)other).AllowBreakBrickBlock);
        }

        public virtual void Interact(bool allow)
        {
            if (allow)
                Remove();
        }
    }
}
