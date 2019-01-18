using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.Items;

namespace SuperMario
{
    public class Player : Character
    {
        //player constants
        const float VELOCITY_WALK_X = 6;
        const float VELOCITY_RUN_X = 12;
        const float VELOCITY_HOLD_X = 7;
        const float STOP_ACCEL = .5f;
        const float ACCEL = .3f;
        const float JUMP_INIT_VELOCITY_Y = 11.5f;
        const float JUMP_BOOST_TIME = .3f;
        const float INVULNERABILITY_TIME = 3;

        internal override float WalkingSpeed => VELOCITY_WALK_X;
        internal override float RunningSpeed => VELOCITY_RUN_X;
        internal override float HoldingSpeed => VELOCITY_HOLD_X;
        internal override float JumpForce => JUMP_INIT_VELOCITY_Y;
        internal override bool RunningAllowed => canRun;
        internal override bool JumpingAllowed => overrideAllowJump ? overrideAllowJump : base.JumpingAllowed;
        internal override bool AllowBreakBrickBlock => PUState != PowerupState.SM_REG;

        public enum PowerupState
        {
            SM_REG,
            REG,
            FIRE,
            STAR
        }
        internal PowerupState PUState;

        public static bool PLAYER_MOVED = false;

        public override string TextureName => "spritesheet";
        bool Invulnerable = false;

        internal enum AnimationDescription : int
        {
            SMALL_WALK = 0,
            TALL_WALK = 2,
            JUMP = 5,
            FIRE_WALK = 4
        }

        /// <summary>
        /// The velocity one frame ago
        /// </summary>
        Vector2 PrevVelocity;        

        bool canRun = false;

        public bool DEBUG_Player
        {
            get; private set;
        }

        public static Player DebugPlayer()
        {
            var rect = new Rectangle(0, 0, 50, 100);
            var p = new Player(rect);
            p.DEBUG_Player = true;
            return p;
        }

        public Player(Rectangle Hitbox) : base(Hitbox)
        {
            DEBUG_Player = false;
            DefaultTextureClip = false;
            ChangePowerupState(PowerupState.SM_REG);
        }

        public override void Load()
        {
            base.Load();
            Animation = new Spritesheet(Texture, 6, 7, 7, 7, 8, 8, 8);
        }        

        public override void Harm()
        {
            if (Invulnerable)
                return;
            base.Harm();
            Invulnerable = true;
            switch (PUState)
            {
                case PowerupState.SM_REG:
                    Die();
                    break;
                default:
                    ChangePowerupState(PowerupState.SM_REG);
                    break;
            }
        }

        public void ChangePowerupState(PowerupState newState)
        {
            PUState = newState;
            switch(newState)
            {
                case PowerupState.SM_REG:
                    Location += new Vector2(0, 50);
                    CameraFollowPoint = new Vector2(25, 15);
                    Height = 45;
                    break;
                case PowerupState.REG:
                    Location += new Vector2(0, -50);
                    CameraFollowPoint = new Vector2(25, 25);
                    Height = 90;
                    break;
            }            
        }

        public override void CollidedInto(Collidable.CollisionType type, Collidable col, GameObject other)
        {
            if (other is Items.Item)
            {
                ((Item)other).Interact(this);
            }
        }

        bool _animationAlternate = false;
        TimeSpan _invulnerabilityTimer;
        void HandleAnimation(GameTime gameTime)
        {
            animationTimer += gameTime.ElapsedGameTime;
            DefaultSource = false;
            //Get animation row in spritesheet by powerup state
            int GetRow()
            {
                switch (PUState)
                {
                    case PowerupState.REG:
                        return (int)AnimationDescription.TALL_WALK;
                    case PowerupState.FIRE:
                        return (int)AnimationDescription.FIRE_WALK;
                    default:
                        return (int)AnimationDescription.SMALL_WALK;
                }
            }
            switch (currentMovement)
            {
                case MovementMode.STILL:                    
                    Source = Animation.GetFrame(GetRow(), 0);
                    break;
                case MovementMode.WALKING:
                case MovementMode.RUNNING:
                    float interval = .05f;
                    if (animationTimer.TotalSeconds < interval)
                        return;
                    animationTimer = TimeSpan.Zero;
                    Animation.Row = GetRow();
                    switch (Animation.Column)
                    {
                        case 0:
                        case 1:
                            Source = Animation.AdvanceFrame();
                            _animationAlternate = false;
                            break;
                        case 2:
                            if (_animationAlternate)
                                Source = Animation.AdvanceFrame(-1);
                            else
                                Source = Animation.AdvanceFrame();
                            break;
                        case 3:
                            Source = Animation.AdvanceFrame(-1);
                            _animationAlternate = true;
                            break;
                    }
                    break;
                case MovementMode.AIR:
                    Source = Animation.GetFrame(GetRow(), (int)AnimationDescription.JUMP);
                    break;
            }
            TextureColor = Color.White;
            if (Invulnerable)
                TextureColor *= _invulnerabilityTimer.Milliseconds % 2 == 0 ? 1 : .5f;
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            HandleAnimation(gameTime);
            VerifyMovement();
            GetInputs(keyboard, gameTime);
            HandleInvulnerabilty(gameTime);
            base.Update(gameTime);
            PrevVelocity = Velocity;
        }

        void HandleInvulnerabilty(GameTime gameTime)
        {
            if (Invulnerable)
            {
                _invulnerabilityTimer += gameTime.ElapsedGameTime;
                Invulnerable = _invulnerabilityTimer.TotalSeconds <= INVULNERABILITY_TIME;
            }
            else
                _invulnerabilityTimer = TimeSpan.Zero;
            DisableEnemyHitDetection = Invulnerable;
        }

        bool overrideAllowJump;
        TimeSpan jumpTimer;

        public void GetInputs(KeyboardState keyboard, GameTime gameTime)
        {            
            canRun = false;
            var keyPresses = Core.ControlHandler.GetKeyControl(keyboard);
            foreach (var k in keyPresses)
                switch (k)
                {
                    case "left":
                        Acceleration.X = -ACCEL;
                        break;
                    case "right":
                        Acceleration.X = ACCEL;
                        break;
                    case "jump":
                        if (jumpTimer.TotalSeconds < JUMP_BOOST_TIME && Velocity.Y <= 0)
                        {
                            if (overrideAllowJump == false && JumpingAllowed)                            
                                overrideAllowJump = true;                                                            
                            Jump();
                        }
                        else
                        {
                            overrideAllowJump = false;
                            jumpTimer = TimeSpan.Zero;
                        }
                        break;
                    case "sprint":
                        canRun = true;
                        break;
                }
            if (overrideAllowJump)
                jumpTimer += gameTime.ElapsedGameTime;
            if (keyPresses.Count == 0)
            {
                if (currentMovement != MovementMode.STILL && currentMovement != MovementMode.AIR)
                {
                    if (Math.Abs(Velocity.X) - STOP_ACCEL < 0)
                    {
                        Velocity.X = 0;
                        Acceleration.X = 0;
                    }
                    else
                        Acceleration.X = ((Velocity.X > 0) ? -1 : 1) * STOP_ACCEL;
                }
                else
                    Acceleration.X = 0;
            }
            else PLAYER_MOVED = true;
        }

        public override void Draw(SpriteBatch sb)
        {
            effects = Velocity.X > 0 ? SpriteEffects.None : (Velocity.X == 0) ? effects : SpriteEffects.FlipHorizontally;
            base.Draw(sb);
        }
    }
}
