using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;

namespace SAM
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;

        Texture2D texture;

        Texture2D mSpriteTexture;
        Vector2 spritePosition;
        Vector2 spriteSpeed = new Vector2(100.0f, 100.0f);

        private bool wasContinuePressed;
        private GamePadState gamePadState;
        private KeyboardState keyboardState;
        private TouchCollection touchState;

        // For rendering the XAML onto a texture
        UIElementRenderer elementRenderer;

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            //timer.UpdateInterval = TimeSpan.FromTicks(333333);

            // Using TimeSpan.Zero causes the update to happen 
            // at the actual framerate of the device. This makes 
            // animation much smoother. However, this will cause 
            // the speed of the app to vary from device to device 
            // where a fixed UpdateInterval will not.
            timer.UpdateInterval = TimeSpan.Zero;
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            // Use the LayoutUpdate event to know when the page layout 
            // has completed so we can create the UIElementRenderer
            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            // Create the UIElementRenderer to draw the XAML page to a texture.

            // Check for 0 because when we navigate away the LayoutUpdate event
            // is raised but ActualWidth and ActualHeight will be 0 in that case.
            if (ActualWidth > 0 && ActualHeight > 0 && elementRenderer == null)
            {
                elementRenderer = new UIElementRenderer(this, (int)ActualWidth, (int)ActualHeight);
            }
        }
        private const Buttons JumpButton = Buttons.A;
        protected void HandleInput()
        {
            keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            touchState = TouchPanel.GetState();
            bool continuePressed =
            keyboardState.IsKeyDown(Keys.Space) ||
            gamePadState.IsButtonDown(Buttons.A);
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                Console.Write("b");
            //Exit();

            if (continuePressed)
                Console.Write("s");
            //input slected
        }

        //private void GetInput(
        //   KeyboardState keyboardState,
        //   GamePadState gamePadState,
        //   TouchCollection touchState,
        //   AccelerometerState accelState,
        //   DisplayOrientation orientation)
        //{
        //    // Get analog horizontal movement.
        //    movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

        //    // Ignore small movements to prevent running in place.
        // if (Math.Abs(movement) < 0.5f)
        //        movement = 0.0f;

        //    // Move the player with accelerometer
        //    if (Math.Abs(accelState.Acceleration.Y) > 0.10f)
        //    {
        //        // set our movement speed
        //        movement = MathHelper.Clamp(-accelState.Acceleration.Y * AccelerometerScale, -1f, 1f);

        //        // if we're in the LandscapeLeft orientation, we must reverse our movement
        //        if (orientation == DisplayOrientation.LandscapeRight)
        //            movement = -movement;
        //    }

        //    // If any digital horizontal movement input is found, override the analog movement.
        //    if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
        //        keyboardState.IsKeyDown(Keys.Left) ||
        //        keyboardState.IsKeyDown(Keys.A))
        //    {
        //        movement = -1.0f;
        //    }
        //    else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
        //             keyboardState.IsKeyDown(Keys.Right) ||
        //             keyboardState.IsKeyDown(Keys.D))
        //    {
        //        movement = 1.0f;
        //    }

        //    // Check if the player wants to jump.
        //    isJumping =
        //        gamePadState.IsButtonDown(JumpButton) ||
        //        keyboardState.IsKeyDown(Keys.Space) ||
        //        keyboardState.IsKeyDown(Keys.Up) ||
        //        keyboardState.IsKeyDown(Keys.W) ||
        //        touchState.AnyTouch();
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            if (null == texture)
            {
                mSpriteTexture = contentManager.Load<Texture2D>("teeworld");

                // Start with the red rectangle
                texture = mSpriteTexture;
            }
            spritePosition.X = 0;
            spritePosition.Y = 0;

            // TODO: use this.content to load your game content here

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            HandleInput();
            // TODO: Add your update logic here
            spritePosition += spriteSpeed * (float)e.ElapsedTime.TotalSeconds;

            int MinX = 0;
            int MinY = 0;
            int MaxX = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - mSpriteTexture.Width;
            int MaxY = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - mSpriteTexture.Height;

            // Check for bounce.
            if (spritePosition.X > MaxX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MaxX;
            }

            else if (spritePosition.X < MinX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MinX;
            }

            if (spritePosition.Y > MaxY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MaxY;
            }
            else if (spritePosition.Y < MinY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MinY;
            }
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            // Render the Silverlight controls using the UIElementRenderer
            elementRenderer.Render();

            // Draw the sprite
            spriteBatch.Begin();

            // Draw the rectangle in its new position
            spriteBatch.Draw(texture, spritePosition, Microsoft.Xna.Framework.Color.White);

            // Using the texture from the UIElementRenderer, 
            // draw the Silverlight controls to the screen
            spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);

            spriteBatch.End();
        }

        private void PhoneApplicationPage_MouseEnter(object sender, MouseEventArgs e)
        {
        }
    }
}