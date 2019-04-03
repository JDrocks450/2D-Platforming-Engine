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
    public struct Statistics
    {
        public int frame;
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Core : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public const int SCRWIDTH = 1024, SCRHEIGHT = 768;

        public static LevelLoader.LevelData levelData;

        public static int WORLD_BOTTOM = 1200;

        public static bool DEBUG = false;
        public static Texture2D BaseTexture;

        public static Controls ControlHandler;

        public static ContentManager Manager;

        public static Camera GameCamera;

        public static bool RESTART_FLAG = false;

        public static Statistics Stats = new Statistics();

        public static Screen CurrentScreen
        {
            get;
            set;
        }

        public static Player ControlledPlayer
        {
            get => GameObjects.OfType<Player>()?.First() ?? null;
        }

        public static int Lives = 5;
        public static int Coins = 0;

        public static SpriteFont Font;

        public static Point MousePosition;

        public static string Dir = Path.Combine(Environment.CurrentDirectory, "Content");

        public static List<UI.UIComponent> UIElements = new List<UI.UIComponent>();

        /// <summary>
        /// Represents a safely editable list of objects that won't crash the game if changed.
        /// </summary>
        public static List<GameObject> GameObjects = new List<GameObject>();
        /// <summary>
        /// An array of objects that should only ever be read from.
        /// </summary>
        public static GameObject[] SafeObjects;

        public static bool MouseVisible = false;

        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Manager = Content;
            graphics.PreferredBackBufferWidth = SCRWIDTH;
            graphics.PreferredBackBufferHeight = SCRHEIGHT;
            graphics.SynchronizeWithVerticalRetrace = false;
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
            UIElements.Add(new Main_Menu(this));
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
            Font = Content.Load<SpriteFont>("Fonts/Font");
            ((Screen)UIElements[0]).Load(Content);
            UI.Tooltip.Load(Content, SCRWIDTH, SCRHEIGHT, Core.BaseTexture);                        
        }

        public void PostMainMenu()
        {
            //Init.
            UISafeElements = new List<UI.UIComponent>();
            UIElements = new List<UI.UIComponent>();
            SafeObjects = new GameObject[0];
            GameObjects.Clear();
            //Load                        
            levelData = LevelLoader.LevelData.LoadFile();
            if (CurrentScreen is LevelCreator)
                ((LevelCreator)CurrentScreen).LoadLevel(levelData);
            CurrentScreen.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            CurrentScreen?.OnExiting();
        }

        static List<UI.UIComponent> UISafeElements = new List<UI.UIComponent>();
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MousePosition = GameCamera.Screen.Location + Mouse.GetState().Position;
            UISafeElements = UIElements.ToList();
            CurrentScreen?.Update(gameTime);
            IsMouseVisible = MouseVisible;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (!UISafeElements.OfType<Main_Menu>().Any())
                {
                    UIElements.Add(new Main_Menu(this));
                    ((Screen)UIElements.Last()).Load(Manager);
                }
            }
            if (RESTART_FLAG)
                Exit();
            foreach (var uielement in UISafeElements)
                uielement.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(CurrentScreen?.Background ?? Color.Black);
            spriteBatch.Begin(CurrentScreen?.SortMode ?? SpriteSortMode.FrontToBack, null, SamplerState.LinearWrap, null, null, null, GameCamera.Transform(GraphicsDevice)); //Repeating texture objects drawn here
            if (!(CurrentScreen is UI.UIComponent))
                CurrentScreen?.Draw(spriteBatch);            
            spriteBatch.End();
            spriteBatch.Begin();
            foreach (var element in UISafeElements)
                element.Draw(spriteBatch);
            UI.Tooltip.sDraw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
            Stats.frame++;
        }        
    }
}
