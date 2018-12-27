using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMario.ControlMapper;
using SuperMario.PrefabObjects;
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

        public static int WORLD_BOTTOM = 1000;        

        public static bool DEBUG = true;
        public static Texture2D BaseTexture;

        public static Controls ControlHandler;
        [Obsolete("Use control handler instead, this is used for UserInterface compatibility.")]
        public static InputHelper GlobalInput;

        public static ContentManager Manager;

        public static UserInterface UILayer;

        public static string Dir = Path.Combine(Environment.CurrentDirectory, "Content");        

        /// <summary>
        /// Represents a safely editable list of objects that won't crash the game if changed.
        /// </summary>
        public static List<GameObject> GameObjects = new List<GameObject>();
        /// <summary>
        /// An array of objects that should only ever be read from.
        /// </summary>
        GameObject[] currentObjs;

        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Manager = Content;
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
            UILayer = new UserInterface(Manager, new Point(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height));
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
            GameObjects.Add(new Ground(new Rectangle(0, 400, 500, 0)));
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

            currentObjs = GameObjects.ToArray();
            var result = ControlHandler.MenuKeys(Keyboard.GetState());
            if (result.Contains(Controls.MENUKeys.DEBUG_PLAYER_CREATE))
                GameObjects.Add(Player.DebugPlayer());
            if (result.Contains(Controls.MENUKeys.DEBUG_OBJECT_CREATE))
                GameObjects.Add(GameObject.CreateDebugObject());
            foreach (var obj in currentObjs)
            {
                obj.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BlueViolet);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap); //Repeating texture objects drawn here
            foreach (var obj in currentObjs)
            {
                obj.Draw(spriteBatch);
            }            
            spriteBatch.End();
            spriteBatch.Begin(); //UI drawn last
            UILayer.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }        
    }
}
