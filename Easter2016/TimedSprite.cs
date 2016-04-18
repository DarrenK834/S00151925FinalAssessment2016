using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easter2016
{
    class TimedSprite: DrawableGameComponent
    {
        TimeSpan _activate;
        TimeSpan _survival;
        Vector2 CurrentPosition;
        string Name;
        bool _activated = false;
        public bool Alive = true;

        public TimeSpan Activate
        {
            get
            {
                return _activate;
            }

            set
            {
                _activate = value;
            }
        }

        public TimeSpan Survival
        {
            get
            {
                return _survival;
            }

            set
            {
                _survival = value;
            }
        }

        public TimedSprite(Game g,string name, Vector2 position): base(g)
        {
            Name = name;
            CurrentPosition = position;
            Activate = new TimeSpan(0, 0, 0, 0,Utilities.Utility.NextRandom(1200));
            Survival = Activate + new TimeSpan(0, 0, 0,0, Utilities.Utility.NextRandom(1200));
            g.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > Activate)
                _activated = true;
            if (gameTime.TotalGameTime > Survival)
            {
                Alive = false;
            }
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            //if(Game.Components.Contains(this))
            //Game.Components.Remove(this);
            base.Dispose(disposing);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_activated)
            {
                SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
                sp.Begin();
                sp.Draw(LoadedGameContent.Textures[Name], CurrentPosition, Color.Red);
                sp.End();
            }

            base.Draw(gameTime);
        }
    }
}
