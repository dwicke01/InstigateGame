using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Instigate
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpawnMenu menu;
        MouseState oldState;
        Map map;
        public Camera2d camera;
        Texture2D background;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            
            //graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera2d(GraphicsDevice.Viewport, 800, 480, 4);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            map = new Map(Content.Load<Texture2D>("Space"), Content.Load<Texture2D>("Sprites/laser"));
            background = Content.Load<Texture2D>(@"Space2");
            menu = new SpawnMenu(Content, map, camera);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            Mouse.WindowHandle = Window.Handle;

            MouseState mouseStateCurrent = Mouse.GetState();

            int previousScroll = oldState.ScrollWheelValue;

            int zoomIncrement = 1;

            // Adjust zoom if the mouse wheel has moved
            if (mouseStateCurrent.ScrollWheelValue > previousScroll)
            {
                camera.Zoom += zoomIncrement;
                menu.visible = false;
                menu.deselectFactionButtons();
                menu.bombSpawner = null;
                menu.shipSpawner = null;
            }
            else if (mouseStateCurrent.ScrollWheelValue < previousScroll)
            {
                camera.Zoom -= zoomIncrement;
                menu.visible = false;
                menu.deselectFactionButtons();
                menu.bombSpawner = null;
                menu.shipSpawner = null;
            }

            previousScroll = mouseStateCurrent.ScrollWheelValue;

            // Move the camera when the arrow keys are pressed
            Vector2 movement = Vector2.Zero;
            Viewport vp = GraphicsDevice.Viewport;

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X--;
            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X++;
            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y--;
            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y++;

            camera.Pos += movement * 20;

            // Transform mouse input from view to world position
            Matrix inverse = Matrix.Invert(camera.GetTransformation());
            Vector2 mousePos = Vector2.Transform(
               new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y), inverse);

            map.Update(gameTime);
            if (Mouse.GetState().RightButton == ButtonState.Pressed && oldState.RightButton == ButtonState.Released)
            {
                menu.visible = !menu.visible;
                if (!menu.visible)
                    menu.deselectFactionButtons();
                else
                    camera.Zoom = 4;
            }
            if (menu.visible)
                menu.Update(oldState, camera);
            oldState = Mouse.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            map.Draw(gameTime, spriteBatch);
            menu.Draw(spriteBatch, camera);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        
    }
}
