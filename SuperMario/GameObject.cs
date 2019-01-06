using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class GameObject
    {
        //game constants
        const float MAX_VELOCITY_XY = 50;
        const float MAX_ACCEL_XY = 25;
        const float GRAVITY = .75f;            

        /// <summary>
        /// Called when location changes.
        /// </summary>
        /// <param name="old">Old location</param>
        /// <param name="updated">New location</param>
        /// <param name="IsXChange">null: no change, false: Y changed, true: X changed</param>
        public delegate void LocationChangedHandler(Vector2 old, Vector2 updated, bool? IsXChange);
        public event LocationChangedHandler OnLocationChanged;
        public delegate void SizeChangedHandler(Point old, Point updated, bool? IsWidthChange);
        public event SizeChangedHandler OnSizeChanged;
        public delegate void CollisionDetected(Collidable.CollisionType type, Collidable collision, GameObject other);
        public event CollisionDetected OnCollision;

        public Rectangle Hitbox
        {
            get => new Rectangle((int)_x, (int)_y, Width, Height);
            set
            {
                _x = value.X;
                _y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public int Width
        {
            get => _w;
            set
            {
                var old = _w;
                _w = value;  
                OnSizeChanged?.Invoke(new Point(old, Height), new Point(Width, Height), true);                              
            }
        }

        public int Height
        {
            get => _h;
            set
            {
                var old = _h;
                _h = value;
                OnSizeChanged?.Invoke(new Point(Width, old), new Point(Width, value), true);
            }
        }

        public Vector2 Location
        {
            get
            {
                return new Vector2(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                var old = _x;
                _x = value;
                if (old != value)
                    OnLocationChanged?.Invoke(new Vector2(old, Y), Location, true);                           
            }
        }
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                var old = _y;
                _y = value;
                if (old != value)
                    OnLocationChanged?.Invoke(new Vector2(X, old), Location, true);
            }
        }

        /// <summary>
        /// The amount location changes per frame
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// the amount velocity changes per frame
        /// </summary>
        public Vector2 Acceleration;

        public Texture2D Texture;
        public Color TextureColor = Color.White;
        internal bool DefaultTextureClip = true;
        internal Rectangle _textureClip;
        /// <summary>
        /// Marks this object to have it's texture repeated across its Hitbox.
        /// </summary>
        internal bool IsTextureRepeating = false;
        internal Point RepeatTextureSize;

        internal SpriteEffects effects;
        internal Spritesheet Animation;
        internal TimeSpan animationTimer;

        internal List<Collidable> Collision;
        internal bool PhysicsApplied = true;
        internal bool StandingCollisionOnly = false;
        internal bool DisableEnemyHitDetection = false;
        public bool gravityAirStateChange;
        public bool InAir
        {
            get; internal set;
        }

        private float _x;
        private float _y;
        private int _w;
        private int _h;

        public GameObject(Texture2D texture, Rectangle hitbox)
        {
            if (hitbox != Rectangle.Empty) //Prefabs abuse this
                Hitbox = hitbox;
            _textureClip = hitbox;
            if (texture != null)
                Texture = texture;
            OnLocationChanged += GameObject_OnLocationChanged;
            Collision = Collidable.CreateCollision(this);
            OnCollision += GameObject_OnCollision; ;
        }

        private void GameObject_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            other.CollidedInto(type, collision, this);
        }

        public virtual void CollidedInto(Collidable.CollisionType type, Collidable col, GameObject other)
        {

        }

        public static GameObject CreateDebugObject()
        {
            var o = new GameObject(Core.BaseTexture, new Rectangle(400, 0, 100, 100));
            o.TextureColor = Color.Gray;
            return o;
        }

        private void GameObject_OnLocationChanged(Vector2 old, Vector2 updated, bool? IsXChange)
        {
            //throw new NotImplementedException();
        }

        internal void SetupRepeatingTexture(Point TextureSize)
        {
            RepeatTextureSize = TextureSize;
            IsTextureRepeating = true;
        }

        bool VerifyGravity()
        {
            if (!PhysicsApplied)
                return false;
            gravityAirStateChange = false;
            if (Y >= Core.WORLD_BOTTOM - Height)
            {
                Y = Core.WORLD_BOTTOM - Height;
                return false;
            }
            return true;
        }

        public virtual void Remove()
        {
            Core.GameObjects.Remove(this);
        }

        public void PHYSICS_INVOKECOLLISION(Collidable.CollisionType type, Collidable collidable, GameObject other)
        {
            OnCollision?.Invoke(type, collidable, other);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (VerifyGravity())
                Acceleration.Y = GRAVITY;
            else
            {
                Acceleration.Y = 0;
                Velocity.Y = 0;
            }
            var accel = Acceleration;
            if (Acceleration.X > MAX_ACCEL_XY)
                accel.X = MAX_ACCEL_XY;
            if (Acceleration.Y > MAX_ACCEL_XY)
                accel.Y = MAX_ACCEL_XY;
            Velocity += Acceleration;
            Location += Velocity;
            foreach (var col in Collision)
                if (StandingCollisionOnly)
                {
                    if (col.Type == Collidable.CollisionType.CEILING)
                        col.UpdateCollsion();
                }
                else                
                    col.UpdateCollsion();                
            if (DefaultTextureClip)
            {
                _textureClip = new Rectangle(0, 0, Width, Height);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            switch (IsTextureRepeating)
            {
                case false:
                    sb.Draw(Texture, Hitbox, _textureClip, TextureColor, 0f, Vector2.Zero, effects, 0);
                    break;
                case true:
                    Point repeatAmount = Hitbox.Size / RepeatTextureSize;
                    sb.Draw(Texture, Location, new Rectangle(0, 0, RepeatTextureSize.X * repeatAmount.X,
                        RepeatTextureSize.Y * repeatAmount.Y), TextureColor);
                    break;
            }
            if (Core.DEBUG)
            {
                sb.Draw(Core.BaseTexture, Hitbox, Color.Orange * .5f);
                foreach (var col in Collision)
                    col.DrawDEBUGCollision(sb);
            }
        }
    }
}
