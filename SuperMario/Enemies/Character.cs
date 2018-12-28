using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class Character : PrefabObjects.Prefab
    {
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
        internal virtual bool JumpingAllowed
        {
            get => !InAir;
        }

        public enum MovementMode
        {
            STILL,
            WALKING,
            RUNNING,
            HOLDING,
            HOLDSTILL
        }
        public MovementMode currentMovement = MovementMode.STILL;

        public Character(Rectangle box) : base(box)
        {
            
        }

        internal void VerifyMovement()
        {
            if (X < Core.GameCamera.Screen.X)
            {
                X = Core.GameCamera.Screen.X;
                this.Velocity.X = 0;
            }
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
                if (Velocity.X > 0)
                    if (Velocity.X <= WalkingSpeed)
                        currentMovement = MovementMode.WALKING;
                    else if (Velocity.X <= RunningSpeed)
                    {
                        if (!RunningAllowed)
                            currentMovement = MovementMode.WALKING;
                        else
                            currentMovement = MovementMode.RUNNING;
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
        }

        /// <summary>
        /// Performs a jump if possible
        /// </summary>
        internal void Jump()
        {
            if (JumpingAllowed)
            {
                Y -= .1f;
                Velocity.Y = -JumpForce;
            }
        }        
    }
}
