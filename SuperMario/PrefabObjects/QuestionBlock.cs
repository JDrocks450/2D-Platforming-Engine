using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class QuestionBlock : Block
    {
        public override string TextureName => !Opened ? "question" : "unbreakable";
        public override Point PreferredSize => new Point(50);

        bool Opened = false;
        
        public enum SpawnObjectLogic
        {
            /// <summary>
            /// Spawns an object based off Mario's PUState
            /// </summary>
            Inferred,
            /// <summary>
            /// Spawns any object selected
            /// </summary>
            Literal
        }

        internal SpawnObjectLogic spawnLogic = SpawnObjectLogic.Literal;
        internal Items.Item StoredItem;
        public bool HasItem => StoredItem != null;

        public QuestionBlock(Rectangle box, SpawnObjectLogic logic, Items.Item spawn = null) : base(box)
        {
            spawnLogic = logic;
            StoredItem = spawn;            
        }        

        

        public override void LoadFromFile(char[] rawBlockData)
        {
            var b = byte.Parse(new string(rawBlockData), System.Globalization.NumberStyles.HexNumber);
            StoredItem = Items.Item.Parse(b, out bool logic);
            spawnLogic = logic ? SpawnObjectLogic.Inferred : SpawnObjectLogic.Literal;
        }

        public override char[] GetBlockData
        {
            get
            {
                byte b = 0;
                for (byte i = 1; i < 10; i++)
                {
                    if (StoredItem == null)
                    {
                        if (spawnLogic == SpawnObjectLogic.Inferred)
                            b = (byte)Items.Item.ITEM_TABLE.QUES_INFER_FLAG;
                        else
                            b = (byte)Items.Item.ITEM_TABLE.NULL;
                        break;
                    }
                    else if (Items.Item.Parse(i, out _).GetType() == StoredItem.GetType())
                    {
                        b = i;
                        break;
                    }
                }
                return b.ToString("X4").ToCharArray();
            }
        }

        public override void Interact(bool allow = true)
        {
            if (Opened)
                return;
            Opened = true;
            if (spawnLogic == SpawnObjectLogic.Inferred)
                StoredItem = Items.Item.GetInferredItem();
            if (HasItem)
                StoredItem.Spawn(this.Location, new Vector2(0,-10));
            Load();
        }
    }
}
