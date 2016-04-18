using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easter2016
{
    class Player : SimpleSprite
    {
        MouseState previousMouseState;
        int AllowedCannonBalls = 2;  
        
        public Player(Game g, string SpriteName, Vector2 StartPosition) 
                        : base(g,SpriteName,StartPosition)
        {
            Active = true;
            speed = 5.0f;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public bool firing()
        {
            var fireCount = Game.Components.OfType<SimpleSprite>()
                .Where(s => !s.Stopped() && s.Name == "pokeball")
                .ToList().Count();

            if (fireCount > AllowedCannonBalls)
                return true;

            return false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Currentposition += new Vector2(-1, 0) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Currentposition += new Vector2(1, 0) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Currentposition += new Vector2(0, 1) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Currentposition += new Vector2(0, -1) * speed;

            Currentposition = Vector2.Clamp(Currentposition,Vector2.Zero, 
                GraphicsDevice.Viewport.Bounds.Size.ToVector2() - 
                new Vector2(LoadedGameContent.Textures[Name].Width, 
                LoadedGameContent.Textures[Name].Height));

            if (!firing() && currentMouseState.LeftButton == ButtonState.Released
                && previousMouseState.LeftButton == ButtonState.Pressed 
                && Game.GraphicsDevice.Viewport.Bounds.Contains(currentMouseState.Position))
            {
               SimpleSprite cannonball = 
                    new SimpleSprite(Game, 
                                    "pokeball",
                                    Currentposition);

                cannonball.Active = true;
               LoadedGameContent.Sounds["cannon fire"].Play();
                // Sets movement target to Mouse Position
                cannonball.moveTo(Mouse.GetState().Position.ToVector2());
            }
            previousMouseState = currentMouseState;
        }
    }
}
