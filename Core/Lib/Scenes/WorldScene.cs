﻿using System.Linq;
using Core.Lib.Entities.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LDtk;

namespace Core.Lib.Scenes
{
    
    public class WorldScene: BaseScene
    {
        private GameLevel[] _levels;
        private int _activeLevel;
        private GameLevel ActiveLevel => _levels[_activeLevel];

        private readonly string worldName;

        public WorldScene(Vector2 dimensions, string worldName): base(dimensions)
        {
            this.worldName = worldName;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ActiveLevel.Draw(spriteBatch);
        }

        public override void Update(float deltaTime)
        {
            ActiveLevel.Update(deltaTime);

        }

        public override void Start()
        {
            new BasePlayer(SceneManager.EntityRendererRegistry).SwitchLevel(ActiveLevel);
            ActiveLevel.Start(SceneManager.CameraController);
        }

        protected override void LoadContent(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            var world = contentManager.Load<LDtkWorld>("GameWorld");
            world.GraphicsDevice = spriteBatch.GraphicsDevice;
            world.spriteBatch = spriteBatch;
            _levels = world.LoadLevels().Select(level => new GameLevel(level)).ToArray();

            for (var i = 0; i < _levels.Length; i++)
            {
                var gameLevel = _levels[i];
                gameLevel.Load(contentManager);
                if (gameLevel.GetId().Equals("Start"))
                {
                    _activeLevel = i;
                }
            }
        }
    }
}