using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Instigate
{
    class SpawnMenu : Microsoft.Xna.Framework.Game
    {
        //public Vector2 position { get; set; }

        List<ShipSpawnButton> shipSpawnButtons; // was a list in case I wanted to add more ships. never got around to it
        BombSpawnButton bombSpawnButton;
        private FactionButton[,] factionButtons;
        public Texture2D sprite;
        public bool visible { get; set; }
        public ShipSpawner shipSpawner;
        public BombSpawner bombSpawner;
        public Vector2 location;
        Map map;

        public SpawnMenu(ContentManager Content, Map map, Camera2d camera)
        {
            sprite = Content.Load<Texture2D>(@"SpawnMenu/Menu2");
            factionButtons = new FactionButton[2,2];
            factionButtons[0, 0] = new FactionButton(Content, 1, 380, Color.Blue, camera);
            factionButtons[0, 1] = new FactionButton(Content, 1, 430, Color.Red, camera);
            factionButtons[1, 0] = new FactionButton(Content, 51, 380, Color.Green, camera);
            factionButtons[1, 1] = new FactionButton(Content, 51, 430, Color.Orange, camera);
            fillShipSpawnButtons(Content, camera);
            shipSpawner = null;
            this.map = map;
            bombSpawnButton = new BombSpawnButton(Content.Load<Texture2D>(@"SpawnMenu/Glow2"), Content.Load<Texture2D>(@"SpawnMenu/Bomb2"), Vector2.Transform(new Vector2(201, 380), camera.GetTransformation()));
            bombSpawner = null;
            location = Vector2.Transform(new Vector2(0, 380), camera.GetTransformation());
        }

        void fillShipSpawnButtons(ContentManager Content, Camera2d camera)
        {
            shipSpawnButtons = new List<ShipSpawnButton>();
            shipSpawnButtons.Add(new ShipSpawnButton(Content.Load<Texture2D>(@"SpawnMenu/Glow2"),Content.Load<Texture2D>(@"SpawnMenu/Ship2"), Vector2.Transform(new Vector2(101, 380), camera.GetTransformation())));
            
        }

        public void setShipSpawnButtons(Color color)
        {
            foreach (ShipSpawnButton button in shipSpawnButtons)
                button.color = color;
        }

        public void onRightClick()
        {
            if (!visible)
                visible = true;
            else
                visible = false;
        }

        public void Draw(SpriteBatch spriteBatch, Camera2d camera)
        {
            if (visible)
            {
                Vector2 mouseTransformed = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(camera.GetTransformation()));

                spriteBatch.Draw(sprite, getRectangle(), Color.White);
                foreach (FactionButton button in factionButtons)
                    button.Draw(spriteBatch);
                foreach (ShipSpawnButton button in shipSpawnButtons)
                    button.Draw(spriteBatch);
                if (shipSpawner != null && IsMouseInsideWindow(camera, mouseTransformed))
                    shipSpawner.Draw(spriteBatch);
                else if (bombSpawner != null && IsMouseInsideWindow(camera, mouseTransformed))
                    bombSpawner.Draw(spriteBatch);

                bombSpawnButton.Draw(spriteBatch);
                
            }
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)location.X, (int)location.Y, sprite.Width, sprite.Height);
        }

        // deselects the faction buttons and sets the spawners to null
        public void deselectFactionButtons()
        {
            bombSpawnButton.color = Color.White;
            bombSpawnButton.selected = false;

            foreach (ShipSpawnButton button in shipSpawnButtons)
            {
                button.color = Color.White;
                button.selected = false;
            }

            foreach (FactionButton button in factionButtons)
            {
                if (button.glow)
                    button.onClick();
            }
            shipSpawner = null;
            bombSpawner = null;
            IsMouseVisible = true;
        }

        public void Update(MouseState oldState, Camera2d camera)
        {
                bool isSelected = false;
            
                location = Vector2.Transform(new Vector2(0, 380), Matrix.Invert(camera.GetTransformation()));
                foreach (FactionButton button in factionButtons)
                    button.Update(camera);
                
                shipSpawnButtons[0].location = Vector2.Transform(new Vector2(101,380), Matrix.Invert(camera.GetTransformation()));
                bombSpawnButton.location = Vector2.Transform(new Vector2(201, 380), Matrix.Invert(camera.GetTransformation()));

                // update methods are passed the camera so they can be positioned relative to the view
                if (shipSpawner != null)
                    shipSpawner.Update(camera);

                if (bombSpawner != null)
                    bombSpawner.Update(camera);

                // used because the menu is relative to the view
                Vector2 mouseTransformed = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(camera.GetTransformation()));
                
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
                {
                    foreach (FactionButton button in factionButtons)
                    {
                        if (button.getRectangle().Contains(new Point((int)mouseTransformed.X, (int)mouseTransformed.Y)))
                        {
                            isSelected = button.glow;
                            deselectFactionButtons();
                            if (!isSelected)
                            {
                                button.onClick();
                                foreach (ShipSpawnButton button2 in shipSpawnButtons)
                                    button2.color = button.getColor();
                                bombSpawnButton.color = button.getColor();
                            }
                        }
                    }

                    if (bombSpawnButton.color!=Color.White)
                    {
                        foreach (ShipSpawnButton button in shipSpawnButtons)
                        {
                            if (button.getRectangle().Contains(new Point((int)mouseTransformed.X, (int)mouseTransformed.Y)))
                            {
                                shipSpawner = button.onClick();
                                bombSpawner = null;
                                bombSpawnButton.selected = false;
                                if (shipSpawner != null)
                                    IsMouseVisible = false;
                                else
                                    IsMouseVisible = true;
                            }
                        }

                        if (bombSpawnButton.getRectangle().Contains(new Point((int)mouseTransformed.X, (int)mouseTransformed.Y)))
                        {
                            bombSpawner = bombSpawnButton.onClick();
                            shipSpawner = null;
                            foreach (ShipSpawnButton button in shipSpawnButtons)
                                button.selected = false;
                            if (bombSpawner != null)
                                IsMouseVisible = false;
                            else
                                IsMouseVisible = true;
                        }
                    }

                    if (IsMouseInsideWindow(camera, mouseTransformed) && !getRectangle().Contains((int)mouseTransformed.X, (int)mouseTransformed.Y) && shipSpawner != null)
                    {
                        map.createShip((int)mouseTransformed.X, (int)mouseTransformed.Y, shipSpawner);
                    }
                    else if (IsMouseInsideWindow(camera, mouseTransformed) && !getRectangle().Contains((int)mouseTransformed.X, (int)mouseTransformed.Y) && bombSpawner != null)
                    {
                        map.createBomb((int)mouseTransformed.X, (int)mouseTransformed.Y, bombSpawner);
                    }
                }
        }

        // checks if the mouse is inside the window
        public bool IsMouseInsideWindow(Camera2d camera, Vector2 mouseTransformed)
        {

            Rectangle rect = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

            return rect.Contains((int)mouseTransformed.X, (int)mouseTransformed.Y);
        }
    }
}
