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
