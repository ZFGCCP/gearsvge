﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Text;
using Gears.Playable;
using Gears.Playable.Projectile;
using Gears.Cloud;
using Gears.Cloud.Input;
using Gears.Navigation;

namespace GearsDebug.Playable.RadialAssault
{
    internal sealed class RAPlayer : Player
    {
        private Vector2 originOfCircle = new Vector2(ViewportHandler.GetWidth() / 2, ViewportHandler.GetHeight() / 2);
        private float theta = MathHelper.TwoPi;
        private float imageOffset;
        private float x = 0;
        private float y = 0;
        private float deltaScalar = MathHelper.PiOver4 / 32;
        private float radius;

        private bool _playerHasControl;
        private bool _isAlive;

        //LASERS
        private List<IProjectile> _projectiles = new List<IProjectile>(); // in testing
        float laserSpeed = 7.5f;
        float distanceToCannon = 15.0f;

        //Texture stuff
        private string fileloc = @"RadialAssault\spaceship32shaded";
        protected override string TextureFileLocation { get { return fileloc; } }

        internal RAPlayer(Vector2 origin, Color color, float rotation)
            : base(origin, color, rotation) 
        {
            InitializeLocal();
        }


        private void InitializeLocal()
        {
            this._imageOrigin = getTextureOrigin();

            imageOffset = 300 + this._imageOrigin.Y; //testing, but fits the screen nicely.
            //imageOffset = 196 + this._imageOrigin.Y; //old, for 64x64
            radius = (ViewportHandler.GetHeight() / 2) + imageOffset;// bad chris
        }

        public override void onUpdate(GameTime gameTime)
        {
            //base.onFrame();
            if (_projectiles != null)
            {
                foreach (IProjectile projectile in _projectiles)
                {
                    projectile.Update(gameTime);
                }
            }
        }

        public override void onDraw(SpriteBatch spriteBatch)
        {
            //base.onDraw();
            if (_projectiles != null)
            {
                foreach (IProjectile projectile in _projectiles)
                {
                    projectile.Draw(spriteBatch);
                }
            }
        }

        internal void MoveClockwise()
        {
            RotateAroundOrigin(true);
        }
        internal void MoveCounterClockwise()
        {
            RotateAroundOrigin(false);
        }

        private void RotateAroundOrigin(bool isClockwise)
        {
            if (isClockwise)
            {
                theta -= deltaScalar; // - is clockwise
                this._rotation += deltaScalar;// + is for clockwise
            }
            else //isCounterClockwise
            {
                theta += deltaScalar; // + is counterclockwise.
                this._rotation -= deltaScalar;// - is for counterclockwise
            }
            float sinTheta = (float)Math.Sin(theta);
            float cosTheta = (float)Math.Cos(theta);

            x = imageOffset * sinTheta + originOfCircle.X;
            y = imageOffset * cosTheta + originOfCircle.Y;

            this._position.X = x;
            this._position.Y = y;
        }
        
        //not implemented appropriately, just here temporarily.
        public Vector2 getTextureOrigin()
        {
            float halfHeight = this._texture.Height / 2.0f;
            float halfWidth = this._texture.Width / 2.0f;

            return new Vector2(halfWidth, halfHeight);
        }

        internal void Activate()
        {
            ActivateInputHooks();
        }
        private void ActivateInputHooks()
        {
            Master.GetInputManager().GetCurrentInputHandler().SubscribeInputHook(KeyDown);
        }
        /// <summary>
        /// Event based Input hook for RAPlayer.
        /// </summary>
        /// <param name="currentKeyboardState">Passed from Input class.</param>
        /// <param name="oldKeyboardState">Passed from Input class.</param>
        internal void KeyDown(ref KeyboardState currentKeyboardState, ref KeyboardState oldKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                MoveClockwise();
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                MoveCounterClockwise();
            }
            if (currentKeyboardState.IsKeyDown(Keys.Space) &&
                currentKeyboardState.IsKeyDown(Keys.Space) != oldKeyboardState.IsKeyDown(Keys.Space))
            {
                FireLasers();
            }
        }

        private void FireLasers()
        {
            //Fire two lasers, one from each cannon.

            Vector2 velocity = new Vector2((float)Math.Cos(MathHelper.ToRadians(270.0f) + _rotation), (float)Math.Sin(MathHelper.ToRadians(270.0f) + _rotation));

            Vector2 right = this._position + Vector2.Transform(velocity * distanceToCannon, Matrix.CreateRotationZ(MathHelper.ToRadians(30.0f)));
            Vector2 left = this._position + Vector2.Transform(velocity * distanceToCannon, Matrix.CreateRotationZ(MathHelper.ToRadians(-30.0f)));

            _projectiles.Add(new LaserBeam(right, velocity * laserSpeed, this));
            _projectiles.Add(new LaserBeam(left, velocity * laserSpeed, this));
        }
        
    }
}
