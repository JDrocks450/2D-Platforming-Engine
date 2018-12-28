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
        public Block(Rectangle Box) : base(Box)
        {
            PhysicsApplied = false;
            OnCollision += Block_OnCollision;
        }

        private void Block_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            if (other == Core.ControlledPlayer && type == Collidable.CollisionType.CEILING) //Player hits bottom            
                Interact();            
        }

        public virtual void Interact()
        {
            Remove();
        }
    }
}
