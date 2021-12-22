using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace First
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //ScreenScale
        private int _screenWidth = 1280;
        private int _screenHeight = 720;
        private int _virtualScreenWidth = 1920;
        private int _virtualScreenHeight = 1080;
        private float _scaleX;
        private float _scaleY;
        private Matrix _matrix;

        //Helpers
        private Random _random;
        private bool _debug = false;

        //Sprites
        private Texture2D _characterSprite;
        private Texture2D _backgroundSprite;
        private Texture2D _targetSprite;
        private Texture2D _textureBlack;

        private SpriteFont _textSprite;

        private Vector2 _targetPosition;
        private float _targetScale = 0.2f;
        private float _targetRadius = 0;

        private MouseState _mouseState;
        private bool _mouseLeftButtonReleased = true;
        private float _mouseTargetDistance = 0;
        private bool _mouseRightButtonReleased = true;

        private int _score = 0;
        private float _timer = 0f;

        public Game1()
        {
            Content.RootDirectory = "Content";
            _random = new Random();

            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 120.0f);
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.ApplyChanges();

            //Apply virtual scale
            _scaleX = (float)_screenWidth / _virtualScreenWidth;
            _scaleY = (float)_screenHeight / _virtualScreenHeight;
            _matrix = Matrix.CreateScale(_scaleX, _scaleY, 1.0f);

            //Creating black texture
            int size = 20;
            _textureBlack = new Texture2D(_graphics.GraphicsDevice, size, size);
            //the array holds the color for each pixel in the texture
            Color[] data = new Color[size * size];
            for(int pixel=0;pixel<data.Count();pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = new Color(55,70,55);
            }
            _textureBlack.SetData(data);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _characterSprite = Content.Load<Texture2D>("Keqing");
            _backgroundSprite = Content.Load<Texture2D>("Background");
            _targetSprite = Content.Load<Texture2D>("Target");
            _textSprite = Content.Load<SpriteFont>("Text");
            
            _targetRadius = (_targetSprite.Height / 2) * _targetScale * _scaleY;
            _targetPosition = new Vector2(_random.Next((int)(int)(_targetRadius / _scaleX), _virtualScreenWidth - (int)(int)(_targetRadius / _scaleX)), _random.Next((int)(int)(_targetRadius / _scaleY), _virtualScreenHeight - (int)(int)(_targetRadius / _scaleY)));
        }

        protected override void Update(GameTime gameTime)
        {   
            var keyboard = Keyboard.GetState();
            _mouseState = Mouse.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if(_timer < 20)
            {
                _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                _targetRadius = (_targetSprite.Height / 2) * _targetScale * _scaleY;

                _mouseTargetDistance = Vector2.Distance(
                    new Vector2(_targetPosition.X * _scaleX, _targetPosition.Y * _scaleY),
                    _mouseState.Position.ToVector2()
                );
                
                //Check left mouse button click once
                if(_mouseState.LeftButton == ButtonState.Released)
                {
                    _mouseLeftButtonReleased = true;
                }
                else if(_mouseState.LeftButton == ButtonState.Pressed && _mouseLeftButtonReleased == true)
                {
                    if(_targetRadius > _mouseTargetDistance)
                    {
                        _score++;
                        if(_targetRadius/1.66 > _mouseTargetDistance)
                        _score++;
                        if(_targetRadius/8.2 > _mouseTargetDistance)
                        _score++;
                        _targetPosition.X = _random.Next((int)(_targetRadius / _scaleX), _virtualScreenWidth - (int)(_targetRadius / _scaleX));
                        _targetPosition.Y = _random.Next((int)(_targetRadius / _scaleY), _virtualScreenHeight - (int)(_targetRadius / _scaleY));
                    }
                    else
                    {
                        _score--;
                    }

                    _mouseLeftButtonReleased = false;
                }
            
                //Check right mouse button click once
                if(_mouseState.RightButton == ButtonState.Released)
                {
                    _mouseRightButtonReleased = true;
                }
                else if(_mouseState.RightButton == ButtonState.Pressed && _mouseRightButtonReleased == true)
                {
                    _score--;
                    _mouseRightButtonReleased = false;
                }

                if(keyboard.IsKeyDown(Keys.D) && _targetPosition.X <= _virtualScreenWidth - _targetRadius / _scaleX)
                    _targetPosition.X += (int)(1000 * gameTime.ElapsedGameTime.TotalSeconds); 
                if(keyboard.IsKeyDown(Keys.A) && _targetPosition.X >= 0 + _targetRadius / _scaleX)
                    _targetPosition.X -= (int)(1000 * gameTime.ElapsedGameTime.TotalSeconds); 
                if(keyboard.IsKeyDown(Keys.W) && _targetPosition.Y >= 0 + _targetRadius / _scaleY)
                    _targetPosition.Y -= (int)(1000 * gameTime.ElapsedGameTime.TotalSeconds); 
                if(keyboard.IsKeyDown(Keys.S) && _targetPosition.Y <= _virtualScreenHeight - _targetRadius / _scaleY)
                    _targetPosition.Y += (int)(1000 * gameTime.ElapsedGameTime.TotalSeconds); 

                if(keyboard.IsKeyDown(Keys.OemPlus) && _targetScale < 2)
                    _targetScale += 0.005f; 

                if(keyboard.IsKeyDown(Keys.OemMinus) && _targetScale > 0.1f)
                    _targetScale -= 0.005f; 
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.RosyBrown);

            _spriteBatch.Begin(transformMatrix: _matrix);
            //_spriteBatch.Begin();
            
            _spriteBatch.Draw(_backgroundSprite, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_characterSprite, new Vector2(1200, 100), Color.White);
            if(_timer < 20) _spriteBatch.Draw(_targetSprite, _targetPosition, null, Color.White, 0f, new Vector2(_targetSprite.Height/2, _targetSprite.Width/2), _targetScale, SpriteEffects.None, 0f );

            _spriteBatch.DrawString(_textSprite, $"TIMER: {_timer.ToString("0.0")}", new Vector2(10, 10), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_textSprite, $"SCORE: {_score}", new Vector2(10, 50), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_textSprite, $"SCORE/SECOND: {(_score/_timer).ToString("0.0")}", new Vector2(10, 90), Color.GreenYellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if(_debug)
            {
                _spriteBatch.DrawString(_textSprite, $"Mouse Distance: {_mouseTargetDistance}", new Vector2(10, 790), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_textSprite, $"Target Radius: {_targetRadius}", new Vector2(10, 830), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_textSprite, $"X: {_targetPosition.X} | {((float)_targetSprite.Width/(float)2 * _targetScale * _scaleX)}", new Vector2(10, 870), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_textSprite, $"Y: {_targetPosition.Y} | {((float)_targetSprite.Width/(float)2 * _targetScale * _scaleY)}", new Vector2(10, 910), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                _spriteBatch.DrawString(_textSprite, $"Mouse: {_mouseState.X} | {_mouseState.Y}", new Vector2(10, 940), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            var size = 20;
            var circle = new Rectangle((int)(_mouseState.X / _scaleX) - size/2, (int)(_mouseState.Y / _scaleY) - size/2, size, size);

            _spriteBatch.Draw(_textureBlack, circle, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
