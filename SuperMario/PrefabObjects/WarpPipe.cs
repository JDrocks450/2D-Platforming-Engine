using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.PrefabObjects
{
    public class WarpPipe : Prefab
    {
        public override string TextureName => "warp1";
        public override Point PreferredSize => new Point(100, 100);
        public override Point IconSize => new Point(50, 50);        

        const float SECONDS_WAIT = .5f;

        static int GetIndentifier()
        {
            var r = Core.GameObjects.OfType<WarpPipe>();
            var w = r.Where(o => o.Identifier == r.Select(x => x.Identifier).Max())?.First();
            int i = 0;
            if (w != null)
            {
                if (w.GetOther == null)
                    i = w.Identifier;
                else
                    i = w.Identifier + 1;
            }
            return i;
        }

        public bool InUse
        {
            get; private set;
        } = false;

        public bool IsRecieveing
        {
            get; private set;
        } = false;

        public int Identifier
        {
            get;
            private set;
        } = -1;

        public WarpPipe GetOther
        {
            get
            {
                var r = Core.SafeObjects.OfType<WarpPipe>()?.Where(x => x != this && x.Identifier == Identifier);
                if (r.Any())
                {
                    return r.First();
                }
                return null;
            }
        }

        public WarpPipe(Point Location) : base(new Microsoft.Xna.Framework.Rectangle(Location, new Point(0)))
        {
            OnCollision += WarpPipe_OnCollision;
            ZIndex = .6f;
        }

        private void WarpPipe_OnCollision(Collidable.CollisionType type, Collidable collision, GameObject other)
        {
            if (type == Collidable.CollisionType.FLOOR)
                if(other == Core.ControlledPlayer )
                    if (Core.ControlHandler.GetKeyControl().Contains("down"))
                    {
                        InUse = true;
                    }                        
        }

        public override void LoadFromFile(char[] rawBlockData)
        {
            Identifier = int.Parse(rawBlockData[0].ToString());
        }

        public override char[] GetBlockData => new char[1] { Identifier.ToString()[0] };

        public override void Update(GameTime gameTime)
        {
            if (Identifier == -1)
                Identifier = GetIndentifier();
            base.Update(gameTime);
            if (InUse)
            {
                if (IsRecieveing)
                {
                    _waitTimer += gameTime.ElapsedGameTime;
                    if (_waitTimer.TotalSeconds < SECONDS_WAIT)
                        return;
                }
                if (GetOther is null)
                {
                    InUse = false;
                    return;
                }
                var play = Core.ControlledPlayer;
                play.LimitedCollision = false;
                play.Velocity = new Vector2(0);
                play.Acceleration = new Vector2(0);
                Core.GameCamera.Focus = this;
                int loc = (int)play.X + play.Source.Center.X;
                if (loc != Hitbox.Center.X)
                {
                    if (loc < Hitbox.Center.X)
                        play.X++;
                    if (loc > Hitbox.Center.X)
                        play.X--;
                    return;
                }
                if (!IsRecieveing)
                {
                    if (play.Y < Y)
                        play.Y += 2;
                    else
                    {
                        _waitTimer += gameTime.ElapsedGameTime;
                        if (_waitTimer.TotalSeconds < SECONDS_WAIT)
                            return;
                        TeleportToOther();
                    }
                }
                else
                {
                    if (play.Y > Y - play.Source.Height)
                        play.Y -= 2;
                    else
                    {
                        InUse = false;
                        play.LimitedCollision = true;
                        Core.GameCamera.Focus = Core.ControlledPlayer;
                        IsRecieveing = false;
                    }
                }
            }
            else
                _waitTimer = TimeSpan.Zero;
        }

        TimeSpan _waitTimer;

        public void Recieve()
        {
            InUse = true;
            IsRecieveing = true;            
        }

        public void TeleportToOther()
        {
            var other = GetOther;
            Core.ControlledPlayer.X = other.X + Core.ControlledPlayer.Source.Center.X;
            Core.ControlledPlayer.Y = other.Y;
            InUse = false;
            other.Recieve();
        }
    }
}
