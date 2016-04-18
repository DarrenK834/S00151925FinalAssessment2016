using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easter2016
{
    class SimpleSpriteManager: DrawableGameComponent
    {
        SoundEffectInstance _audioPlayer;
        List<SimpleSprite> _blackKnights = new List<SimpleSprite>();
        LinkedList<TimedSprite> timed = new LinkedList<TimedSprite>();
        TimeSpan TimePassed;
        Player player;
        Tower startTower;
        Tower playerTower;

        public SimpleSpriteManager(Game g) : base(g)
        {
            g.Components.Add(this);
        }

        protected override void LoadContent()
        {
            LoadAssets();
            setupObjects();
            for (int i = 0; i < 5; i++)
            {
                TimedSprite next = new TimedSprite(Game, "pokeball", new Vector2(Utilities.Utility.NextRandom(200), Utilities.Utility.NextRandom(200)));
                addtoTimes(next);
            }

            base.LoadContent();
        }

        private void addtoTimes(TimedSprite timedSprite)
        {
            if (timed.Count == 0)
                timed.AddFirst(timedSprite);
            else
            {
                LinkedListNode<TimedSprite> current = timed.First;
                while (current != timed.Last && timedSprite.Activate >= current.Value.Activate)
                    current = current.Next;
                if (current == timed.Last && timedSprite.Activate >= current.Value.Activate)
                    timed.AddAfter(timed.First, timedSprite);
                else
                    timed.AddBefore(current, timedSprite);
            }

        }

        private void removeSimpleSpriteComponents()
        {
            var removalList = Game.Components.OfType<SimpleSprite>().Where(s => s.Alive == false).ToList();
            if (removalList.Count() > 0)
                foreach (SimpleSprite deadCharacter in removalList)
                    Game.Components.Remove(deadCharacter);
        }

        private void setupObjects()
        {

            // Players Tower is bottom left
            Vector2 PlayerTowerPos = new Vector2(0,
            GraphicsDevice.Viewport.Height 
                  - LoadedGameContent.Textures["PokiCenter"].Height
            
            );

            // Player is placed bottom left of the Viewport
            Vector2 playerPosition = PlayerTowerPos + new Vector2(LoadedGameContent.Textures["character1"].Width,
                        -LoadedGameContent.Textures["character1"].Height
                        ) ;

            Vector2 startTowerPos = new Vector2(GraphicsDevice.Viewport.Width - LoadedGameContent.Textures["PokemonGym"].Width,
            0
            );

            SimpleSprite background =  new SimpleSprite(Game, "minecraft", Vector2.Zero);
            background.Active = true;

            player = new Player(Game, "character1", playerPosition);
            playerTower = new Tower(Game, "PokiCenter", PlayerTowerPos );
            startTower = new Tower(Game, "PokemonGym", startTowerPos);

            for (int i = 0; i < 5; i++)
            {
                Stack<Vector2> path = new Stack<Vector2>();
                path.Push(PlayerTowerPos);
                path.Push(new Vector2(Utilities.Utility.NextRandom(200), Utilities.Utility.NextRandom(400)));

                SimpleSprite s = new SimpleSprite(Game, "enemy1", startTowerPos, path);
                _blackKnights.Add(s);
                
            }
            _blackKnights.First().Active = true;
            _blackKnights.First().followPath();
        }

        public  void monitorKnights()
        {
            // if they are not all stopped then there is at least one active
            var _activeKnights = _blackKnights
                .Where(k => !k.Stopped() && k.Active);
            if (_activeKnights.Count() < 1)
            {
                // then the inactive one has been deleted so activate the next one and add
                // a new one
                Vector2 startTowerPos = new Vector2(GraphicsDevice.Viewport.Width - LoadedGameContent.Textures["PokemonGym"].Width,
                            0);
                Vector2 target = new Vector2(0,
                                    GraphicsDevice.Viewport.Height - LoadedGameContent.Textures["PokiCenter"].Height
                                    );
                // Add a new one
                Stack<Vector2> path = new Stack<Vector2>();
                path.Push(target);
                path.Push(new Vector2(Utilities.Utility.NextRandom(200), Utilities.Utility.NextRandom(400)));
                SimpleSprite s = new SimpleSprite(Game, "enemy1", startTowerPos, path);
                _blackKnights.Add(s);
                // acticate the next one at the head of the list
                _blackKnights.First().Active = true;
                _blackKnights.First().followPath();
            }
            else 
            {
                // Check for collision with the tower 
                // NOTE: we only delete the first one.
                // Subsequent updates will call this again and delete others one at a time
                // otherwise we get a iteration error over active knights as the referennce to the object disappears 
                // When removed from the _blackknoghts collection and the Game Component collection
                foreach (var enemy in _activeKnights)
                {
                    if (playerTower.Collision(enemy))
                    {
                        LoadedGameContent.Sounds["Impact"].Play();
                        Game.Components.Remove(enemy);
                        _blackKnights.Remove(enemy);
                        break;
                   }
                }

            }
        }

        private void LoadAssets()
        {
            LoadedGameContent.Sounds.Add("backing", Game.Content.Load<SoundEffect>("Backing Track wav"));
            LoadedGameContent.Sounds.Add("cannon fire", Game.Content.Load<SoundEffect>("cannon fire"));
            LoadedGameContent.Sounds.Add("Impact", Game.Content.Load<SoundEffect>("Impact"));
            LoadedGameContent.Textures.Add("enemy1", Game.Content.Load<Texture2D>("enemy1"));
            LoadedGameContent.Textures.Add("pokeball", Game.Content.Load<Texture2D>("pokeball"));
            LoadedGameContent.Textures.Add("PokemonGym", Game.Content.Load<Texture2D>("PokemonGym"));
            LoadedGameContent.Textures.Add("PokiCenter", Game.Content.Load<Texture2D>("PokiCenter"));
            LoadedGameContent.Textures.Add("minecraft",Game.Content.Load<Texture2D>("minecraft"));
            LoadedGameContent.Textures.Add("character1", Game.Content.Load<Texture2D>("character1"));
            LoadedGameContent.Fonts.Add("SimpleSpriteFont", Game.Content.Load<SpriteFont>("SimpleSpriteFont"));

            _audioPlayer = LoadedGameContent.Sounds["backing"].CreateInstance();
            _audioPlayer.Volume = 0.2f;
            _audioPlayer.IsLooped = true;
            _audioPlayer.Play();

        }

        public void MonitorCannonBalls()
        {
            // remove any cannon all that is not moving
            var removalList = Game.Components.OfType<SimpleSprite>()
                .Where(s => s.Stopped() && s.Name == "pokeball").ToList();
            if(removalList.Count() > 0)
                LoadedGameContent.Sounds["Impact"].Play();
            foreach (var item in removalList)
                Game.Components.Remove(item);
            // get the active cannon balls
            var activeCannonBalls = Game.Components.OfType<SimpleSprite>()
                .Where(s => !s.Stopped() && s.Name == "pokeball").ToList();
            // Get the active enemies
            var enemies = Game.Components.OfType<SimpleSprite>()
                .Where(s => !s.Stopped() && s.Name == "enemy1").ToList();

            // check collisions between cannon balls and enemies
            foreach (var b in activeCannonBalls)
            {
                foreach (var enemy in enemies)
                {
                    if (b.Collision(enemy))
                    {
                        LoadedGameContent.Sounds["Impact"].Play();
                        Game.Components.Remove(b);
                        Game.Components.Remove(enemy);
                        _blackKnights.Remove(enemy);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            TimePassed = gameTime.TotalGameTime;
            checkTimedObjects();
            MonitorCannonBalls();
            monitorKnights();
            base.Update(gameTime);
        }

        private void checkTimedObjects()
        {
            var deadTimed = Game.Components.OfType<TimedSprite>()
                            .Where(t => !t.Alive).ToList();
            foreach (TimedSprite t in deadTimed)
            {
                timed.Remove(t);
                Game.Components.Remove(t);
            }
            if(timed.Count < 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    TimedSprite next = new TimedSprite(Game, "pokeball", new Vector2(Utilities.Utility.NextRandom(200), Utilities.Utility.NextRandom(200)));
                    next.Activate += TimePassed;
                    next.Survival += TimePassed;
                    addtoTimes(next);
                }

            }

        }
    }
}
