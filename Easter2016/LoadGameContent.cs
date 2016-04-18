using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Easter2016
{
    public static class LoadedGameContent 
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
        public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();

        

        public static Dictionary<String, T> ContentLoad<T>(ContentManager Content, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            Dictionary<String, T> result = new Dictionary<String, T>();
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[key] = Content.Load<T>(contentFolder + "\\" + key);
            }
            return result;
        }
    }
}
