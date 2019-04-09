using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Items
{
    public abstract class Item : PrefabObjects.Prefab
    {
        public override Point IconSize => new Point(50);
        public override string IconName => TextureName;

        public enum ITEM_TABLE : byte
        {
            NULL,
            MUSHROOM,
            FIREFLOWER,
            COIN,
            EXTRA_LIFE,
            /// <summary>
            /// sets a flag in question blocks to infer the object to spawn.
            /// </summary>
            QUES_INFER_FLAG = 9
        }

        public Item(Rectangle box) : base(box, false)
        {

        }

        public void Spawn(Vector2 Location, Vector2 Velocity)
        {
            Load();
            this.Location = Location;
            this.Velocity = Velocity;
            Acceleration.X = 0;
            ZIndex = 0;
            Core.GameObjects.Add(this);
        }

        public override void CollidedInto(Collidable.CollisionType type, Collidable col, GameObject other)
        {
            if (other is Player)
                Interact(other as Player);
        }

        public abstract void Interact(Player other);

        public static Item GetInferredItem()
        {
            switch (Core.ControlledPlayer.PUState)
            {
                case Player.PowerupState.SM_REG:
                    return new Mushroom();
                case Player.PowerupState.REG:
                    return new FireFlower();
                default:
                    return new Mushroom();
            }
        }

        public static Item Parse(byte savedObjId, out bool isInferred)
        {
            isInferred = false;
            switch ((Items.Item.ITEM_TABLE)savedObjId)
            {
                case Items.Item.ITEM_TABLE.MUSHROOM:
                    return new Items.Mushroom();
                case Items.Item.ITEM_TABLE.FIREFLOWER:
                    return new Items.FireFlower();
                case Items.Item.ITEM_TABLE.QUES_INFER_FLAG:
                    isInferred = true;
                    return null;
                case ITEM_TABLE.COIN:
                    return new Coin();
                default:
                    return null;
            }
        }
    }
}
