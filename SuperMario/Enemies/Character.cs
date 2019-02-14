using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class Character : PrefabObjects.Prefab
    {
        public interface Enemy
        {

        }

        internal virtual float JumpForce
        {
            get => 15;
        }
        internal virtual float WalkingSpeed
        {
            get => 10;
        }
        internal virtual float RunningSpeed
        {
            get => 15;
        }
        internal virtual float HoldingSpeed
        {
            get => 7;
        }
        internal virtual bool RunningAllowed
        {
            get => false;
        }
        internal virtual float StompBoost
        {
            get => 8;
        }
        internal virtual bool JumpingAllowed
        {
            get => !InAir;
        }
        internal virtual bool AllowBreakBrickBlock => true;        

        public bool IsDead
        {
            get; internal set;
        }

        public enum Direction : int
        {
            Left,
            Right
        }
        public Direction Facing = Direction.Left;

        internal override bool OnScreen => X >= Core.GameCamera.Screen.X && X < Core.GameCamera.Screen.Right;         

        public bool ForceUpdate
        {
            get; internal set;
        } = false;

        public enum MovementMode
        {
            STILL,
            WALKING,
            RUNNING,
            HOLDING,
            HOLDSTILL,
            AIR
        }
        public MovementMode currentMovement = MovementMode.STILL;

        internal bool movementVerified = false;

        public Character(Rectangle box) : base(box)
        {
            
        }

        void AdjustCollision()
        {
            foreach(var collisionBox in Collision)
            {
                switch (collisionBox.Type)
                {
                    case Collidable.CollisionType.WALL:
                        collisionBox.Size = Height - Collidable.WALL_OFFSET_Y;
                        break;
                    case Collidable.CollisionType.FLOOR:
                        collisionBox.DeactivateCollisionBox(0);
                        break;
                }
            }
        }

        public virtual void Harm()
        {

        }

        public virtual void Die()
        {
            Remove();
            IsDead = true;
        }

        public override void Update(GameTime gameTime)
        {
            VerifyMovement();
            AdjustCollision();
            base.Update(gameTime);
            movementVerified = false;
        }

        internal void VerifyMovement(bool force = false)
        {
            if (!force)
                if (movementVerified)
                    return;
            bool negativeX = false;
            if (this.Velocity.X == 0 && !InAir)
                currentMovement = MovementMode.STILL;
            var Velocity = this.Velocity; //overshadow public variable with local one
            if (Velocity.X < 0)
            {
                Velocity.X *= -1;
                negativeX = true;
            }
            if (!InAir)
            {
                if (Velocity.X > 0)
                    if (!RunningAllowed)
                        currentMovement = MovementMode.WALKING;
                    else
                        currentMovement = MovementMode.RUNNING;
            }
            else
            {
                Acceleration.X = 0;
                currentMovement = MovementMode.AIR;
            }
            switch (currentMovement)
            {
                case MovementMode.STILL:
                    Velocity.X = 0;
                    Acceleration.X = 0;
                    break;
                case MovementMode.HOLDSTILL:
                    Velocity = Vector2.Zero;
                    Acceleration = Vector2.Zero;
                    break;
                case MovementMode.AIR:
                    if (RunningAllowed)
                    {
                        if (Velocity.X + Acceleration.X > RunningSpeed)
                            Velocity.X = RunningSpeed - Acceleration.X;
                    }
                    else if (Velocity.X + Acceleration.X > WalkingSpeed)
                        Velocity.X = WalkingSpeed - Acceleration.X;
                    break;
                case MovementMode.WALKING:
                    if (Velocity.X + Acceleration.X > WalkingSpeed)
                        Velocity.X = WalkingSpeed - Acceleration.X;
                    break;
                case MovementMode.RUNNING:
                    if (Velocity.X + Acceleration.X > RunningSpeed)
                        Velocity.X = RunningSpeed - Acceleration.X;
                    break;
                case MovementMode.HOLDING:
                    if (Velocity.X + Acceleration.X > HoldingSpeed)
                        Velocity.X = HoldingSpeed - Acceleration.X;
                    break;
            }
            this.Velocity = Velocity;
            if (negativeX)
                this.Velocity.X *= -1;
            movementVerified = true;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        /// <summary>
        /// Performs a jump if possible
        /// </summary>
        internal void Jump(float overrideForce = -1)
        {
            if (JumpingAllowed)
            {
                Y -= .1f;
                Velocity.Y = overrideForce < 0 ? -JumpForce : -overrideForce;
            }
        }        
    }
}
