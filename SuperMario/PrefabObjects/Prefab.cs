﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public abstract class Prefab : GameObject
    {
        public virtual string TextureName
        {
            get;
        }

        public virtual string IconName
        {
            get => TextureName;
        }

        public virtual Point PreferredSize
        {
            get;
        }

        public virtual Point IconSize
        {
            get => PreferredSize;
        }

        /// <summary>
        /// Gets textures by name stored in TextureName property of Prefabs
        /// </summary>
        public static Dictionary<string, Texture2D> LoadedTextures;

        public Prefab(Rectangle Hitbox, bool AutoLoad = true) : base(null, Hitbox)
        {
            if (AutoLoad)
                Load();
        }

        /// <summary>
        /// Loads all premade objects for quick and easy use in levels.
        /// </summary>
        public static void PrefabWarmup()
        {
            LoadedTextures = new Dictionary<string, Texture2D>();
            new Ground(new Rectangle());
        }

        public virtual void Load()
        {
            var name = "Textures/" + TextureName;
            if (!Prefab.LoadedTextures.Keys.Contains(name))
                Prefab.LoadedTextures.Add(name, Core.Manager.Load<Texture2D>(name));
            Texture = LoadedTextures[name];
            if (Width + Height == 0)
            {
                Width = PreferredSize.X;
                Height = PreferredSize.Y;
            }
        }

        public Texture2D GetTexture(string name)
        {
            return LoadedTextures[name];
        }
    }
}
