using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{    
    /// <summary>
    /// This object stretches to the bottom of the screen and uses the default ground texture.
    /// </summary>
    public class Ground : Prefab
    {
        public override string TextureName
        {
            get => "rock";
        }
        public override Point PreferredSize => new Point(50);

        /// <summary>
        /// Height component is automatically adjusted at runtime.
        /// </summary>
        /// <param name="Hitbox"></param>
        public Ground(Rectangle Hitbox) : base(Hitbox)
        {
            SetupRepeatingTexture(new Point(50));
            CalculateCollision = false;
            OnLocationChanged += Ground_OnLocationChanged;
            UpdateHeight(Hitbox.Y);
            //CreateBushes();
        }
        
        void CreateBushes()
        {
            Core.GameObjects.Add(new Bush(Location.ToPoint() + new Point(Width / 2, -Bush.HEIGHT)));
        }

        private void Ground_OnLocationChanged(Vector2 old, Vector2 updated, bool? IsXChange)
        {
            UpdateHeight((int)updated.Y);
        }

        void UpdateHeight(int newY)
        {
            Height = Core.WORLD_BOTTOM - newY;
        }
        public override void Update(GameTime gameTime)
        {            
            base.Update(gameTime);
        }
    }
}
