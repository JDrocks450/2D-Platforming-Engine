using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class Collidable
    {
        const int WALL_BOXSIZE1 = 5;
        const int WALL_BOXSIZE2 = 10;
        const int WALL_DISTANCE = 0;

        const int FLOOR_BOXSIZE1 = 15;
        const int FLOOR_BOXSIZE2 = 10;

        public GameObject Following
        {
            get;
        }

        public enum CollisionType
        {
            WALL,
            CEILING,
            FLOOR
        }
        public CollisionType Type;

        public enum WallDirection
        {
            LEFT,
            RIGHT
        }
        WallDirection Direction;

        Rectangle[] collsionboxes = new Rectangle[2];

        public Vector2 Location;
        public int Size;

        public static List<Collidable> CreateCollision(GameObject obj)
        {
            List<Collidable> collidables = new List<Collidable>();
            for(int i = 0; i <= 3; i++)
            {
                switch (i)
                {
                    case 0: //left
                        collidables.Add(new Collidable(obj.Location, obj.Height, obj, CollisionType.WALL));
                        break;
                    case 2: //floor
                        collidables.Add(new Collidable(obj.Location, obj.Width, obj, CollisionType.FLOOR));
                        break;
                    case 1: //right
                        collidables.Add(new Collidable(new Vector2(obj.Location.X + obj.Width, obj.Location.Y),
                            obj.Height, obj, CollisionType.WALL, WallDirection.RIGHT));
                        break;
                    case 3: //ceiling
                        collidables.Add(new Collidable(new Vector2(obj.Location.X, obj.Location.Y+obj.Height),
                            obj.Width, obj, CollisionType.CEILING));
                        break;
                }
            }
            return collidables;
        }

        public void UpdateLocation()
        {
            var obj = Following;
            switch (Type)
            {
                case CollisionType.WALL:
                    switch (Direction)
                    {
                        case WallDirection.LEFT:
                            Location = obj.Location;
                            break;
                        case WallDirection.RIGHT:
                            Location = new Vector2(obj.Location.X + obj.Width, obj.Location.Y);
                            break;
                    }
                    Size = obj.Height;
                    break;
                case CollisionType.FLOOR:
                    Location = obj.Location;
                    Size = obj.Width;
                    break;
                case CollisionType.CEILING:
                    Location = new Vector2(obj.Location.X, obj.Location.Y + obj.Height);
                    Size = obj.Width;
                    break;
            }
        }

        public Collidable(Vector2 Position, int Length, GameObject follow, CollisionType type, WallDirection direction = WallDirection.LEFT)            
        {
            Location = Position;
            Size = Length;
            Type = type;
            Direction = direction;
            Following = follow;
            if (Following != null)
            {
                Following.OnLocationChanged += Following_OnLocationChanged;
                Following.OnSizeChanged += Following_OnSizeChanged;
            }
        }

        private void Following_OnSizeChanged(Point old, Point updated, bool? IsWidthChange)
        {
            UpdateLocation();
        }

        private void Following_OnLocationChanged(Vector2 old, Vector2 updated, bool? IsXChange)
        {
            var change = updated - old;
            Location += change;
        }

        void Setup()
        {
            var Location = this.Location.ToPoint();
            switch (Type)
            {
                case CollisionType.WALL:
                    collsionboxes[0] = new Rectangle(Location.X - ((Direction == WallDirection.LEFT) ? WALL_BOXSIZE1 : WALL_BOXSIZE2),
                        Location.Y, (Direction == WallDirection.LEFT) ? WALL_BOXSIZE1 : WALL_BOXSIZE2, Size);
                    collsionboxes[1] = new Rectangle(Location.X, Location.Y, (Direction == WallDirection.RIGHT) ? WALL_BOXSIZE1 : WALL_BOXSIZE2, Size);
                    break;
                case CollisionType.FLOOR:
                    collsionboxes[0] = new Rectangle(Location.X, Location.Y - FLOOR_BOXSIZE1, Size, FLOOR_BOXSIZE1);
                    collsionboxes[1] = new Rectangle(Location.X, Location.Y, Size, FLOOR_BOXSIZE2);
                    break;
                case CollisionType.CEILING:
                    collsionboxes[1] = new Rectangle(Location.X, Location.Y - FLOOR_BOXSIZE1, Size, FLOOR_BOXSIZE1);
                    collsionboxes[0] = new Rectangle(Location.X, Location.Y, Size, FLOOR_BOXSIZE2);
                    break;
            }
        }

        public void UpdateCollsion()
        {
            Setup();
            foreach (var obj in Core.SafeObjects)
            {
                var objGrounded = false;
                foreach (var colBox in collsionboxes)
                {
                    if (obj != Following)
                        if (obj.Hitbox.Intersects(colBox))
                        {
                            switch (Type)
                            {
                                case CollisionType.WALL:
                                    switch (Direction)
                                    {
                                        case WallDirection.LEFT:
                                            if (obj.Velocity.X > 0)
                                            {
                                                obj.X = (Location.X - obj.Width) - WALL_DISTANCE;
                                                obj.Velocity.X = 0;
                                            }
                                            break;
                                        case WallDirection.RIGHT:
                                            if (obj.Velocity.X < 0)
                                            {
                                                obj.X = (Location.X) + WALL_DISTANCE;
                                                obj.Velocity.X = 0;
                                            }
                                            break;
                                    }
                                    break;
                                case CollisionType.FLOOR:
                                    if (obj.Velocity.Y >= 0)
                                    {
                                        obj.Y = Location.Y - obj.Height;
                                        obj.Velocity.Y = 0;
                                        objGrounded = true;
                                    }
                                    break;
                                case CollisionType.CEILING:
                                    if (obj.Velocity.Y < 0)
                                    {
                                        obj.Y = Location.Y;
                                        obj.Velocity.Y = 0;
                                    }
                                    break;
                            }
                            Following.PHYSICS_INVOKECOLLISION(Type, this, obj);
                            break;
                        }
                }
                if (!obj.gravityAirStateChange)
                {
                    if (!objGrounded)
                        obj.InAir = true;
                    obj.gravityAirStateChange = true;
                }
                if (objGrounded)
                    obj.InAir = false;
            }
        }

        public void DrawDEBUGCollision(SpriteBatch sb)
        {
            var Location = this.Location.ToPoint();
            var color = Color.Blue; //default color for floor
            switch (Type)
            {
                case CollisionType.FLOOR:
                    sb.Draw(Core.BaseTexture, new Rectangle(Location.X, Location.Y-1, Size, 2), color);
                    break;
                case CollisionType.WALL: //color for wall
                    color = Color.Green;
                    sb.Draw(Core.BaseTexture, new Rectangle(Location.X-1, Location.Y, 2, Size), color);
                    break;
                case CollisionType.CEILING: //color for ceiling
                    color = Color.Orange;
                    sb.Draw(Core.BaseTexture, new Rectangle(Location.X, Location.Y - 1, Size, 2), color);
                    break;
            }            
            color *= .5f; //make color semi-transparent
            foreach (var colBox in collsionboxes)
                sb.Draw(Core.BaseTexture, colBox, color);            
        }
    }
}
