using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easter2016
{
    class Tower : SimpleSprite
    {
        float _damage;

        public Tower(Game g, string SpriteName, Vector2 StartPosition) : base(g,SpriteName,StartPosition)
        {
            Active = true;

        }

        public float Damage
        {
            get
            {
                return _damage;
            }

            set
            {
                _damage = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }

    }
}
