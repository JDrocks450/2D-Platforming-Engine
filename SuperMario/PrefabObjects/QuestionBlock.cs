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

        public QuestionBlock(Rectangle box) : base(box)
        {

        }

        public override void Interact()
        {
            if (Opened)
                return;
            Opened = true;
            Load();
        }
    }
}
