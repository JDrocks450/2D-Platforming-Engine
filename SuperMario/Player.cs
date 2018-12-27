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
    public class Player : PrefabObjects.Prefab
    {
        //player constants
        const float VELOCITY_WALK_X = 10;
        const float VELOCITY_RUN_X = 15;
        const float VELOCITY_HOLD_X = 7;
        const float STOP_ACCEL = .5f;
        const float ACCEL = .5f;
        const float JUMP_INIT_VELOCITY_Y = 10;

        public override string TextureName => "Textures/spritesheet";

        internal enum AnimationDescription : int
        {
            SMALL_WALK = 0,
            TALL_WALK = 2,
            FIRE_WALK = 4
        }

        /// <summary>
        /// The velocity one frame ago
        /// </summary>
        Vector2 PrevVelocity;

        public enum MovementMode
        {
            STILL,
            WALKING,
            RUNNING,
            JUMPING,
            HOLDING,
            HOLDSTILL
        }
        public MovementMode currentMovement = MovementMode.STILL;

        public bool DEBUG_Player
        {
            get; private set;
        }

        public static Player DebugPlayer()
        {
            var rect = new Rectangle(0, 0, 75, 100);
            var p = new Player(rect);
            p.DEBUG_Player = true;
            p.DEBUG_Information = new UserInterface.StackPanel();
            p.DEBUG_Information.AddToParent(Core.UILayer);
            p.TextureColor = Color.SeaGreen;
            return p;
        }

        public Player(Rectangle Hitbox) : base(Hitbox)
        {
            DEBUG_Player = false;
            Collision.Clear();
        }

        public override void Load()
        {
            base.Load();
            Animation = new Spritesheet(Texture, 6, 7, 7, 7, 8, 8, 8);
        }

        void VerifyMovementSpeeds()
        {
            bool negativeX = false;
            if (this.Velocity.X == 0 && currentMovement != MovementMode.JUMPING)
                currentMovement = MovementMode.STILL;
            var Velocity = this.Velocity; //overshadow public variable with local one
            if (Velocity.X < 0) {
                Velocity.X *= -1;
                negativeX = true;
            }
            if (Velocity.X > 0)
                if (Velocity.X <= VELOCITY_WALK_X)
                    currentMovement = MovementMode.WALKING;
                else if (Velocity.X <= VELOCITY_RUN_X)
                    currentMovement = MovementMode.RUNNING;
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
                    if (Velocity.X + Acceleration.X > VELOCITY_WALK_X)
                        Velocity.X = VELOCITY_WALK_X - Acceleration.X;
                    break;
                case MovementMode.RUNNING:
                    if (Velocity.X + Acceleration.X > VELOCITY_RUN_X)
                        Velocity.X = VELOCITY_RUN_X - Acceleration.X;
                    break;
                case MovementMode.HOLDING:
                    if (Velocity.X + Acceleration.X > VELOCITY_HOLD_X)
                        Velocity.X = VELOCITY_HOLD_X - Acceleration.X;
                    break;
                case MovementMode.JUMPING:
                    Velocity.X = PrevVelocity.X;
                    Acceleration.X = 0;
                    break;
            }
            this.Velocity = Velocity;
            if (negativeX)
                this.Velocity.X *= -1;
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
        }

        public override void Update(GameTime gameTime)
        {                       
            var keyboard = Keyboard.GetState();
            VerifyMovementSpeeds();
            GetInputs(keyboard);                       
            base.Update(gameTime);
            if (DEBUG_Player)
                PopulateDebugInfo();
            HandleAnimation(gameTime);
            PrevVelocity = Velocity;
        }

        public void GetInputs(KeyboardState keyboard)
        {
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
                        Y -= .1f;
                        Velocity.Y = -JUMP_INIT_VELOCITY_Y;
                        break;
                }
            if (keyPresses.Count == 0)
                if (currentMovement != MovementMode.STILL)
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
