using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.ControlMapper;
using SuperMario.Enemies;
using SuperMario.PrefabObjects;
using SuperMario.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SuperMario
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Core : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int WORLD_BOTTOM = 700;

        public static bool DEBUG = false;
        public static Texture2D BaseTexture;

        public static Controls ControlHandler;

        public static ContentManager Manager;

        public static Camera GameCamera;

        public Screen CurrentScreen
        {
            get => _curScreen;
            set
            {
                _curScreen = value;
                typeofCurrentScreen = _curScreen.UnderlyingType;
            }
        }
        public static Screen.SCREENS typeofCurrentScreen;

        public static Player ControlledPlayer
        {
            get => (Player)GameObjects.Find(x => x is Player);
        }

        public static string Dir = Path.Combine(Environment.CurrentDirectory, "Content");        

        /// <summary>
        /// Represents a safely editable list of objects that won't crash the game if changed.
        /// </summary>
        public static List<GameObject> GameObjects = new List<GameObject>();
        /// <summary>
        /// An array of objects that should only ever be read from.
        /// </summary>
        public static GameObject[] SafeObjects;
        private Screen _curScreen;

        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Manager = Content;
#if DEBUG
            DEBUG = true;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            BaseTexture = new Texture2D(GraphicsDevice, 1, 1);
            BaseTexture.SetData(new Color[] { Color.White });
            GameCamera = new Camera();
            CurrentScreen = Screen.CreateScreen(Screen.SCREENS.GAME);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ControlHandler = new Controls(Path.Combine(Dir, Controls.XMLNAME));
            PrefabObjects.Prefab.PrefabWarmup();
            CurrentScreen.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CurrentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, GameCamera.Transform(GraphicsDevice)); //Repeating texture objects drawn here
            CurrentScreen.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }        
    }
}
