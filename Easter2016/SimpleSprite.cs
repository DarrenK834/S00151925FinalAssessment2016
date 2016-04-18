using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easter2016
{
    public class SimpleSprite : DrawableGameComponent
    {
        private Vector2 currentposition;
        protected Vector2 _targetposition;
        private Stack<Vector2> path;
        protected bool _alive = true;
        private bool active = false;
        private string name;
        protected float speed = 2.0f;
        Guid id = Guid.NewGuid();
        public Guid Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public bool Alive
        {
            get
            {
                return _alive;
            }

            set
            {
                _alive = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public Stack<Vector2> Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public  bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }

        protected Vector2 Currentposition
        {
            get
            {
                return currentposition;
            }

            set
            {
                currentposition = value;
            }
        }

        public SimpleSprite(Game g, string SpriteName, Vector2 StartPosition, Stack<Vector2> SetPath)
                        : base(g)

        {
            path = SetPath;
            g.Components.Add(this);
            Alive = true;
            Name = SpriteName;
            Currentposition = StartPosition;

        }

        public SimpleSprite(Game g, string SpriteName, Vector2 StartPosition) 
                                : base(g)
        {
            g.Components.Add(this);
            Alive = true;
            Name = SpriteName;
            Currentposition = StartPosition;
            _targetposition = StartPosition;
            path = new Stack<Vector2>();

        }
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                if (Currentposition != _targetposition)
                {
                    Vector2 direction = _targetposition - Currentposition;
                    direction.Normalize();
                    Currentposition += direction * speed;
                    if (Vector2.DistanceSquared(Currentposition, _targetposition) <
                            Vector2.DistanceSquared(Currentposition, Currentposition - direction * speed))
                    {
                        Currentposition = _targetposition;
                        if (path.Count > 0)
                            _targetposition = path.Pop();
                    }
                }
            }
            base.Update(gameTime);
        }
       
        public bool Stopped()
        {
            if (Currentposition == _targetposition)
                return true;
            return false;
        }
       
        protected override void LoadContent()
        {
            base.LoadContent();
        }
        public bool Collision(SimpleSprite other)
        {
            // Translate the rectangle to the current position
            Rectangle thisBound = LoadedGameContent.Textures[Name].Bounds;
            thisBound.X = (int)Currentposition.X;
            thisBound.Y = (int)Currentposition.Y;

            // Translate the rectangle to the current position
            Rectangle otherBound = LoadedGameContent.Textures[Name].Bounds;
            otherBound.X = (int)other.Currentposition.X;
            otherBound.Y = (int)other.Currentposition.Y;
            // Check the collision
            if (thisBound.Intersects(otherBound))
                return true;
            return false;

        }

        public void moveTo(Vector2 NewTarget)
            {
            speed = Math.Abs(speed);
            if (Currentposition == _targetposition)
                _targetposition = NewTarget;
        }

        public void followPath()
        {
            if(path.Count() > 0)
                _targetposition = path.Pop();
        }

        internal bool OutSide()
        {   
            
            Rectangle thisBound = LoadedGameContent.Textures[Name].Bounds;
            thisBound.X = (int)Currentposition.X;
            thisBound.Y = (int)Currentposition.Y;

            if (!Game.GraphicsDevice.Viewport.Bounds.Contains(thisBound))
                return true;

            return false;
        }

        public void bounce()
        {
            speed *= -1;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Active)
            {
                SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
                sp.Begin();
                sp.Draw(LoadedGameContent.Textures[Name], Currentposition, Color.White);
                sp.End();
            }
            base.Draw(gameTime);
        }

        public override string ToString()
        {
            SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
            sp.Begin();
            sp.DrawString(LoadedGameContent.Fonts["SimpleSpriteFont"],
                String.Concat("id: ",id.ToString()," name : ",name), Currentposition + new Vector2(0, -20), Color.White);
            sp.End();

            return base.ToString();
        }
    }
}
