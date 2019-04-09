using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.Items
{
    public class Coin : Item
    {
        public override string TextureName => "CoinSheet";
        public override string IconName => "Icons/coin";

        public Coin(Point Position = default) : base(new Rectangle(Position, new Point(50)))
        {
            DefaultTextureClip = false;
        }

        public override void Load()
        {
            base.Load();
            Animation = new Spritesheet(Texture, 1, 3);
            Source = Animation.GetFrame(0, 0);
            DefaultSource = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (Texture == null)
                Load();
            base.Update(gameTime);
            if (animationTimer >= TimeSpan.FromSeconds(1.0 / 3))
            {
                Source = Animation.AdvanceFrame();
                animationTimer = TimeSpan.FromSeconds(0);
            }
            animationTimer += gameTime.ElapsedGameTime;
        }

        public override void Interact(Player other)
        {
            Core.Coins++;
            Remove();
        }
    }
}
