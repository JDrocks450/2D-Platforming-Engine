using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMario.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class Collidable
    {
        const int WALL_BOXSIZE1 = 2;
        const int WALL_BOXSIZE2 = 10;
        public const int WALL_OFFSET_Y = 5;
        const int WALL_DISTANCE = 0;

        const int FLOOR_OFFSET_X = 2;
        const int FLOOR_BOXSIZE1 = 15;
        const int FLOOR_BOXSIZE2 = 10;

        public static Dictionary<GameObject, int> _updatedObjs;

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
        bool[] activatedBoxes = new bool[] { true, true };

        public Vector2 Location;
        public int Size;

        public static List<Collidable> CreateCollision(GameObject obj)
        {
            if (_updatedObjs is null) _updatedObjs = new Dictionary<GameObject, int>();
            List<Collidable> collidables = new List<Collidable>();
            for (int i = 0; i <= 3; i++)
            {
                switch (i)
                {
                    case 1: //left
                        collidables.Add(new Collidable(obj.Location, obj.Height, obj, CollisionType.WALL));
                        break;
                    case 0: //floor
                        collidables.Add(new Collidable(obj.Location, obj.Width, obj, CollisionType.FLOOR));
                        break;
                    case 2: //right
                        collidables.Add(new Collidable(new Vector2(obj.Location.X + obj.Width, obj.Location.Y),
                            obj.Height, obj, CollisionType.WALL, WallDirection.RIGHT));
                        break;
                    case 3: //ceiling
                        collidables.Add(new Collidable(new Vector2(obj.Location.X, obj.Location.Y + obj.Height),
                            obj.Width, obj, CollisionType.CEILING));
                        break;
                }
            }
            //The above locations are obsolete in favor of UpdateLocation
            foreach (var c in collidables)
                c.UpdateLocation();
            return collidables;
        }

        public void DeactivateCollisionBox(params int[] indices)
        {
            foreach (var index in indices)
                activatedBoxes[index] = false;
        }

        public void ActivateCollisionBox(params int[] indices)
        {
            foreach (var index in indices)
                activatedBoxes[index] = true;
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
                            Location = obj.Location + new Vector2(0, WALL_OFFSET_Y);
                            break;
                        case WallDirection.RIGHT:
                            Location = new Vector2(obj.Location.X + obj.Width, obj.Location.Y + WALL_OFFSET_Y);
                            break;
                    }
                    Size = obj.Height;
                    break;
                case CollisionType.FLOOR:
                    Location = obj.Location + new Vector2(FLOOR_OFFSET_X,0);
                    Size = obj.Width - 2*FLOOR_OFFSET_X;
                    break;
                case CollisionType.CEILING:
                    Location = new Vector2(obj.Location.X, obj.Location.Y + obj.Height) + new Vector2(FLOOR_OFFSET_X, 0);
                    Size = obj.Width - 2 * FLOOR_OFFSET_X;
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

        public static void Final()
        {
            foreach (var obj in Core.SafeObjects)
            {
                obj.gravityAirStateChange = false;                
            }
            _updatedObjs.Clear();
        }

        public bool Setup()
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
            int i = 0;
            var success = true;
            foreach (var b in activatedBoxes)
            {
                if (b == false)
                    collsionboxes[i] = new Rectangle(-1000, 0, 0, 0);
                i++;
            }
            return success;
        }

        bool DoFloorCollision(List<Rectangle> boxes, Collidable r, GameObject obj, out bool success, bool ignoreBoxesCheck = false)
        {
            success = false;            
            if (r.Type != CollisionType.CEILING)
                return false;
            obj.InAir = true;
            if (obj.Velocity.Y >= 0)
            {
                if (!ignoreBoxesCheck)
                    if (obj is Character && obj.InAir)
                        if (boxes.Contains(r.collsionboxes[0]) && boxes.Count() == 1)
                            return false;
                obj.Y = Location.Y - obj.Height;
                obj.Velocity.Y = 0;
                obj.InAir = false;
                obj.gravityAirStateChange = true;
                success = true;
            }
            return true;
        }

        public static IEnumerable<T> Extract<T>(IEnumerable<T> input, out IEnumerable<T> output)
        {
            output = input;
            return input;
        }

        public void UpdateCollsion()
        {
            Setup();
            foreach (var obj in Core.SafeObjects)
            {
                if (obj != Following && obj.LimitedCollision)
                {
                    if (!obj.gravityAirStateChange)
                        obj.InAir = true;
                    foreach (var colBox in collsionboxes)
                    {
                        if ((obj.DisableEnemyHitDetection || Following.DisableEnemyHitDetection) && (obj is Character.Enemy || Following is Character.Enemy))
                            break;
                        var result = obj.Collision.Where(x => x.collsionboxes.Where(y => y.Intersects(colBox)).Any());
                        if (result.Any())
                        {
                            var success = false;
                            foreach (var r in result)
                            {
                                var boxes = r.collsionboxes.Where(y => y.Intersects(colBox)).ToList();
                                switch (Type)
                                {
                                    case CollisionType.WALL:
                                        if (r.Type != CollisionType.WALL)
                                            continue;
                                        switch (Direction)
                                        {
                                            case WallDirection.LEFT:
                                                if (obj.Velocity.X > 0)
                                                {
                                                    obj.X = (Location.X - obj.Width) - WALL_DISTANCE;
                                                    obj.Velocity.X = 0;
                                                    success = true;
                                                }
                                                break;
                                            case WallDirection.RIGHT:
                                                if (obj.Velocity.X < 0)
                                                {
                                                    obj.X = (Location.X) + WALL_DISTANCE;
                                                    obj.Velocity.X = 0;
                                                    success = true;
                                                }
                                                break;
                                        }
                                        break;
                                    case CollisionType.FLOOR:
                                        if (r.Type != CollisionType.CEILING)
                                            continue;
                                        if (!DoFloorCollision(boxes, r, obj, out success))
                                            continue;
                                        break;
                                    case CollisionType.CEILING:
                                        if (r.Type != CollisionType.FLOOR)
                                            continue;
                                        if (obj.Velocity.Y < 0)
                                        {
                                            obj.Y = Location.Y;
                                            obj.Velocity.Y = 1;
                                            success = true;
                                        }
                                        break;
                                }
                                if (success)
                                {
                                    if (Type == CollisionType.FLOOR)
                                        obj.StandingOn = Following;
                                    Following.PHYSICS_INVOKECOLLISION(Type, this, obj);
                                }
                            }
                            break;
                        }
                    }
                }             
            }
        }

        public void DrawDEBUGCollision(SpriteBatch sb)
        {
            var Location = this.Location.ToPoint();
            var color = Color.Blue; //default color for floor
            switch (Type)
            {
                case CollisionType.FLOOR:
                    sb.Draw(Core.BaseTexture, new Rectangle(Location.X, Location.Y-1, Size, 2), null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
                    break;
                case CollisionType.WALL: //color for wall
                    color = Color.Green;
                    sb.Draw(Core.BaseTexture, new Rectangle(Location.X-1, Location.Y, 2, Size), null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
                    break;
                case CollisionType.CEILING: //color for ceiling
                    color = Color.Red;
                    sb.Draw(Core.BaseTexture, new Rectangle(Location.X, Location.Y - 1, Size, 2), null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
                    break;
            }
            if (Following.DisableEnemyHitDetection)
                color = Color.Aqua;
            color *= .5f; //make color semi-transparent
            foreach (var colBox in collsionboxes)
                sb.Draw(Core.BaseTexture, colBox, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);            
        }
    }
}
