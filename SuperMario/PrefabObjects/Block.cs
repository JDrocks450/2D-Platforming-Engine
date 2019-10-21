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
            CalculateCollision = false;
            OnCollision += Block_OnCollision;
        }

        private void Block_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            if (type == Collidable.CollisionType.FLOOR)
                GravityApplied = true;
            if (other == Core.ControlledPlayer && type == Collidable.CollisionType.CEILING) //Player hits bottom  
                Interact(((Player)other).AllowBreakBrickBlock);
        }

        public virtual void Interact(bool allow)
        {
            if (allow)
                Remove();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
