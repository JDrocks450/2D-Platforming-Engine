using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class Camera
    {
        const int DISTANCE_FROM_WORLD_BOTTOM = 100;

        protected float _zoom; // Camera Zoom
        protected float _rotation; //Camera Rotation
        public Matrix _transform; // Matrix Transform

        public bool CanMoveBackwards = true;

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public Vector2 Pos;

        public Rectangle Screen;

        public GameObject Focus
        {
            get; set;
        }

        /// <summary>
        /// reduce camera movement by using one character height for Y calculations.
        /// </summary>
        int charBaseHeight = 0;

        /// <summary>
        /// Creates a matrix for the spritebatch that automatically focuses on the "Focus" if there is one set.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public Matrix Transform(GraphicsDevice graphicsDevice)
        {
            var player = Focus;
            var screen = graphicsDevice.Viewport;
            if (player != null)
            {
                if (charBaseHeight == 0)
                    charBaseHeight = player.Height;
                var center = new Vector2(player.X + (player.Width / 2), (player.Y + (charBaseHeight / 2)));
                center += new Vector2(screen.Width, screen.Height) / 2;
                if (Core.WORLD_BOTTOM - center.Y > DISTANCE_FROM_WORLD_BOTTOM)
                {
                    if (!CanMoveBackwards)
                    {
                        if (center.X > Pos.X)
                            Pos.X = center.X;
                    }
                    else
                        Pos.X = center.X;
                    Pos.Y = center.Y;
                    Screen = new Rectangle((int)Pos.X - screen.Width, (int)Pos.Y - screen.Height, screen.Width, screen.Height);
                }
            }           
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                                         Matrix.CreateRotationZ(0) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(screen.Width, screen.Height, 0));
            return _transform;
        }

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            Pos = Vector2.Zero;
        }
    }
}
