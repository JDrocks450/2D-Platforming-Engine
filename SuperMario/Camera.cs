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
        const int DISTANCE_FROM_WORLD_BOTTOM = 50;
        const int OFFSET_Y = 70;

        protected float _zoom; // Camera Zoom
        protected float _rotation; //Camera Rotation
        public Matrix _transform; // Matrix Transform

        public bool CanMoveBackwards = true;

        public bool CameraIgnoreWorldBottom = false;

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public Vector2 Pos;

        public Rectangle Screen;

        public GameObject Focus
        {
            get => _focus;
            set
            {
                if (value != _focus)
                {
                    _focus = value;
                    MoveToFocus();
                }
            }
        }

        /// <summary>
        /// Hold the camera where it was when this was set to true
        /// </summary>
        public bool HoldingCamera = false;
        /// <summary>
        /// The camera will not follow where the focus is when this is true
        /// </summary>
        public bool PauseFocusFollowing = false;
        int _movingToFocusSpd = 5;
        bool _movingToFocus = false;

        /// <summary>
        /// reduce camera movement by using one character height for Y calculations.
        /// </summary>
        int charBaseHeight = 0;
        private GameObject _focus;

        public void MoveToFocus(int speed = default)
        {
            if (speed != default)
                _movingToFocusSpd = speed;         
            PauseFocusFollowing = true;
            _movingToFocus = true;
        }

        void doMoveToFocus(Vector2 newCenter)
        {
            if (Pos.X != newCenter.X)
            {
                if (Pos.X + _movingToFocusSpd < newCenter.X)
                    Pos.X += _movingToFocusSpd;
                else if (Pos.X - _movingToFocusSpd > newCenter.X)
                    Pos.X -= _movingToFocusSpd;
                else
                    goto cont;
                if (Pos.Y != newCenter.Y)
                    if (Pos.Y+_movingToFocusSpd < newCenter.Y)
                        Pos.Y += _movingToFocusSpd;
                    else if(Pos.Y-_movingToFocusSpd > newCenter.Y)
                        Pos.Y -= _movingToFocusSpd;
                return;
            }
            cont:
            Pos = newCenter;
            _movingToFocus = false;
            PauseFocusFollowing = false;
        }

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
                var center = player.Location + player.CameraFollowPoint;
                center += new Vector2(screen.Width, screen.Height) / 2;
                center.Y -= OFFSET_Y;
                if (_movingToFocus)
                    doMoveToFocus(center);
                int dist = DISTANCE_FROM_WORLD_BOTTOM;
                dist = (CameraIgnoreWorldBottom) ? -9999 : dist;
                if (Core.WORLD_BOTTOM - center.Y > dist && !HoldingCamera)
                {
                    if (!PauseFocusFollowing)
                    {
                        if (!CanMoveBackwards)
                        {
                            if (center.X > Pos.X)
                                Pos.X = center.X;
                        }
                        else
                            Pos.X = center.X;
                        Pos.Y = center.Y;
                    }
                    Screen = new Rectangle((int)Pos.X - screen.Width, (int)Pos.Y - screen.Height, screen.Width, screen.Height);
                }
                else
                {

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
