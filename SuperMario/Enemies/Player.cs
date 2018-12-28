using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperMario
{
    public class Player : Character
    {
        //player constants
        const float VELOCITY_WALK_X = 10;
        const float VELOCITY_RUN_X = 15;
        const float VELOCITY_HOLD_X = 7;
        const float STOP_ACCEL = .5f;
        const float ACCEL = .5f;
        const float JUMP_INIT_VELOCITY_Y = 15;

        internal override float WalkingSpeed => VELOCITY_WALK_X;
        internal override float RunningSpeed => VELOCITY_RUN_X;
        internal override float HoldingSpeed => VELOCITY_HOLD_X;
        internal override float JumpForce => JUMP_INIT_VELOCITY_Y;
        internal override bool RunningAllowed => canRun;
        internal override bool JumpingAllowed => true;

        public override string TextureName => "spritesheet";

        internal enum AnimationDescription : int
        {
            SMALL_WALK = 0,
            TALL_WALK = 2,
            TALL_JUMP = 2,
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
            p.DEBUG_Information = new UserInterface.StackPanel();
            p.DEBUG_Information.AddToParent(Core.UILayer);
            return p;
        }

        public Player(Rectangle Hitbox) : base(Hitbox)
        {
            DEBUG_Player = false;
            Physics_Ghost = true;
        }

        public override void Load()
        {
            base.Load();
            Animation = new Spritesheet(Texture, 6, 7, 7, 7, 8, 8, 8);
        }        

        UserInterface.StackPanel DEBUG_Information;
        void PopulateDebugInfo()
        {
            DEBUG_Information.Components.Clear();
            foreach(var variable in typeof(Player).GetFields(System.Reflection.BindingFlags.Public))
            {
                var text = new InterfaceComponent().CreateText(new InterfaceFont(),
                    $"{variable.Name}, {variable.GetValue(this)}", Color.White);
                text.AddToParent(DEBUG_Information);
            }
        }
        
        bool _animationAlternate = false;
        void HandleAnimation(GameTime gameTime)
        {
            animationTimer += gameTime.ElapsedGameTime;
            DefaultTextureClip = false;
            if (!InAir)
                switch (currentMovement)
                {
                    case MovementMode.STILL:
                        _textureClip = Animation.GetFrame((int)AnimationDescription.TALL_WALK, 0);
                        break;
                    case MovementMode.WALKING:
                    case MovementMode.RUNNING:
                        float interval = .05f;
                        if (animationTimer.TotalSeconds < interval)
                            return;
                        animationTimer = TimeSpan.Zero;
                        Animation.Row = (int)AnimationDescription.TALL_WALK;
                        switch (Animation.Column)
                        {
                            case 0:
                            case 1:
                                _textureClip = Animation.AdvanceFrame();
                                _animationAlternate = false;
                                break;
                            case 2:
                                if (_animationAlternate)
                                    _textureClip = Animation.AdvanceFrame(-1);
                                else
                                    _textureClip = Animation.AdvanceFrame();
                                break;
                            case 3:
                                _textureClip = Animation.AdvanceFrame(-1);
                                _animationAlternate = true;
                                break;
                        }
                        break;
                }
            else
                _textureClip = Animation.GetFrame((int)AnimationDescription.TALL_JUMP, 5);
            Width = _textureClip.Width;
            Height = _textureClip.Height;
        }

        public override void Update(GameTime gameTime)
        {                       
            var keyboard = Keyboard.GetState();
            HandleAnimation(gameTime);                                    
            VerifyMovement();            
            GetInputs(keyboard);                       
            base.Update(gameTime);            
            if (DEBUG_Player)
                PopulateDebugInfo();            
            PrevVelocity = Velocity;
        }

        public void GetInputs(KeyboardState keyboard)
        {
            canRun = false;
            var keyPresses = Core.ControlHandler.GetKeyControl(keyboard);
            var flagForce = false;
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
                        flagForce = true;
                        Jump();
                        break;
                    case "sprint":
                        canRun = true;
                        break;
                }
            if (keyPresses.Count == 0 || flagForce)
                if (currentMovement != MovementMode.STILL && !InAir)
                    Acceleration.X = ((Velocity.X > 0) ? -1 : 1) * STOP_ACCEL;
                else
                    Acceleration.X = 0;                                    
        }

        public override void Draw(SpriteBatch sb)
        {
            effects = Velocity.X > 0 ? SpriteEffects.None : (Velocity.X == 0) ? effects : SpriteEffects.FlipHorizontally;
            base.Draw(sb);
        }
    }
}
