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

        public bool Disabled
        {
            get;
            set;
        }

        /// <summary>
        /// Represents whether or not the camera is focused on the object.
        /// </summary>
        public bool Focused
        {
            get => Core.GameCamera.Focus == this;
        }

        [Obsolete]
        /// <summary>
        /// Works but is slow, try not to use it too often
        /// </summary>
        public string Name => Enum.GetName(typeof(LevelLoader.LevelData.OBJ_TABLE), LevelLoader.LevelData.GetIDByInstance(this));

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

        public float Graphics_Rotation
        {
            get; set;
        } = 0;

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

        internal bool DefaultSource = true;
        /// <summary>
        /// The area on the Spritesheet to use as the drawn sprite.
        /// </summary>
        internal Rectangle Source;
        internal bool DefaultTextureClip = true;
        /// <summary>
        /// A generated box the texture is rendered to
        /// </summary>       
        internal Rectangle TextureClip
        {
            get
            {
                if (DefaultTextureClip)
                    return Hitbox;
                int diffX = (Width - Source.Width)/2;
                int diffY = (Height - Source.Height) / 2;
                return new Rectangle((int)Location.X + diffX, (int)Location.Y + diffY, Source.Width, Source.Height);
            }
        }
        /// <summary>
        /// Marks this object to have it's texture repeated across its Hitbox.
        /// </summary>
        internal bool IsTextureRepeating = false;
        /// <summary>
        /// The amount of times to repeat the texture.
        /// </summary>
        internal Point RepeatTextureSize;

        /// <summary>
        /// Gets or sets whether to use the middle of the object for CameraFollowPoint.
        /// </summary>
        public bool ManualCameraFollowPoint
        {
            get;
            internal set;
        } = false;

        /// <summary>
        /// The point the camera looks at when following this object.
        /// </summary>
        public Vector2 CameraFollowPoint
        {
            get
            {
                if (ManualCameraFollowPoint)
                    return _camFol;
                else
                    return new Vector2(Width / 2, Height / 2);
            }
            internal set
            {
                _camFol = value;
                ManualCameraFollowPoint = true;
            }
        }

        internal SpriteEffects effects;
        internal Spritesheet Animation;
        internal TimeSpan animationTimer;

        /// <summary>
        /// The list of collidables for the object. (almost always 4)
        /// </summary>
        internal List<Collidable> Collision;
        /// <summary>
        /// Object can clip through objects and has no gravity. Other objects cannot pass through it unless they are also not calculating collision.
        /// </summary>
        internal bool CalculateCollision = true;
        internal bool StandingCollisionOnly = false;
        internal bool DisableEnemyHitDetection = false;
        public bool gravityAirStateChange;

        int _inAirFrameChangedAmount = 0;
        /// <summary>
        /// Gets or sets whether the object is in the air. (has no effect on gravity)
        /// </summary>
        public bool InAir
        {
            get => _inAir;
            internal set
            {
                if (value != _inAir)
                {
                    _inAir = value;
                    _inAirFrameChangedAmount++;
                }
            }
        }
        /// <summary>
        /// Object can clip through objects and other objects can also clip though it.
        /// </summary>
        public bool CollisionGhosted
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets whether the object's collision has been updated this frame.
        /// </summary>
        public bool Physics_CollisionUpdatedOnFrame
        {
            get; set;
        }
        /// <summary>
        /// Gets the last object stood on.
        /// </summary>
        public GameObject StandingOn
        {
            get; internal set;
        }
        /// <summary>
        /// Gets the last object stood on returns null if in air.
        /// </summary>
        /// <returns></returns>
        internal GameObject GetObjectRestingOn()
        {
            if (!InAir)
                return StandingOn;
            return null;
        }
        /// <summary>
        /// gets or sets whether the object is on screen and is able to be drawn and updated.
        /// </summary>
        internal virtual bool OnScreen
        {
            get
            {
                return Hitbox.Right >= Core.GameCamera.Screen.X && X < Core.GameCamera.Screen.Right + 50;
            }
        }

        public virtual bool ForceUpdate { get; set; } = false;

        private float _x;
        private float _y;
        private int _w;
        private int _h;
        private Vector2 _camFol;
        private bool _inAir;

        public GameObject(Texture2D texture, Rectangle hitbox)
        {
            if (hitbox != Rectangle.Empty) //Prefabs abuse this
                Hitbox = hitbox;
            Source = hitbox;
            if (texture != null)
                Texture = texture;
            OnLocationChanged += GameObject_OnLocationChanged;
            Collision = Collidable.CreateCollision(this);
            OnCollision += GameObject_OnCollision; ;
        }

        /// <summary>
        /// The raw data after the required objData that the object manages internally.
        /// </summary>
        /// <param name="rawBlockData"></param>
        public virtual void LoadFromFile(char[] rawBlockData)
        {

        }
        /// <summary>
        /// The raw block data saved after the required objData.
        /// </summary>
        public virtual char[] GetBlockData
        {
            get => new char[0];
        }
        public float ZIndex { get; set; } = .5f;
        public bool GravityApplied { get; set; } = true;

        private void GameObject_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            other.CollidedInto(type, collision, this);
        }

        public virtual void CollidedInto(Collidable.CollisionType type, Collidable col, GameObject other)
        {

        }

        public void DumpDebugInfo()
        {
            System.Diagnostics.Debug.WriteLine(Core.Stats.frame + " || InAir changed: " + _inAirFrameChangedAmount);
            _inAirFrameChangedAmount = 0;
        }

        public static GameObject CreateDebugObject()
        {
            var o = new GameObject(Core.BaseTexture, new Rectangle(400, 0, 100, 100));
            o.TextureColor = Color.Gray;
            return o;
        }

        private void GameObject_OnLocationChanged(Vector2 old, Vector2 updated, bool? IsXChange)
        {
            
        }

        internal void SetupRepeatingTexture(Point TextureSize)
        {
            RepeatTextureSize = TextureSize;
            IsTextureRepeating = true;
        }

        bool VerifyGravity()
        {
            if (!CalculateCollision)
                return false;
            if (!GravityApplied)
                return false;
            if (Y >= Core.WORLD_BOTTOM - Height)
            {
                Y = Core.WORLD_BOTTOM - Height;
                if (this is Character)
                    (this as Character).Die();
                else
                    Remove();
                return false;
            }
            return true;
        }

        public virtual void Remove()
        {
            Core.GameObjects.Remove(this);
            if (Focused)
                Core.GameCamera.Focus = null;
        }

        public void PHYSICS_INVOKECOLLISION(Collidable.CollisionType type, Collidable collidable, GameObject other)
        {
            OnCollision?.Invoke(type, collidable, other);
        }        

        public virtual void Update(GameTime gameTime)
        {
            if (VerifyGravity())
                Acceleration.Y = GRAVITY;
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
                    {
                        col.UpdateCollsion();
                    }
                }
                else
                    col.UpdateCollsion();                
            if (DefaultSource)
            {
                Source = new Rectangle(0, 0, Texture.Width, Texture.Height);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            switch (IsTextureRepeating)
            {
                case false:
                    sb.Draw(Texture, 
                        new Rectangle(TextureClip.Location + (Graphics_Rotation != 0 ? (TextureClip.Size / new Point(2)) : new Point(0)), TextureClip.Size), 
                        Source, TextureColor, Graphics_Rotation, Graphics_Rotation != 0 ? 
                        new Vector2(Source.Width/2, Source.Height/2) : Vector2.Zero, effects, ZIndex);
                    break;
                case true:
                    Point repeatAmount = Hitbox.Size / RepeatTextureSize;
                    sb.Draw(Texture, Location, new Rectangle(0, 0, RepeatTextureSize.X * repeatAmount.X,
                        RepeatTextureSize.Y * repeatAmount.Y), TextureColor, Graphics_Rotation, Vector2.Zero, 1f, effects, ZIndex);
                    break;
            }
            if (Core.DEBUG)
            {
                sb.Draw(Core.BaseTexture, Hitbox, null, Color.Orange * .5f, 0, Vector2.Zero, SpriteEffects.None, 1);
                foreach (var col in Collision)
                    col.DrawDEBUGCollision(sb);
            }
        }
    }
}
