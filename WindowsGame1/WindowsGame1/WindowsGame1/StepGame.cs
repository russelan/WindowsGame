using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace StepGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class StepGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Model ship;

        /// <summary>
        /// Current angle in radians
        /// </summary>
        private float angle = 0;

        /// <summary>
        /// Rotation speed in radians per second
        /// </summary>
        private float rotationSpeed = 3.14f;

        /// <summary>
        /// Current ship position
        /// </summary>
        private Vector3 position = new Vector3(0, 0, 0);

        //FOR FINAL PROJECT
        public enum GameState{Splash, Game, End}
        GameState gameState;
        private Texture2D SpriteTexture;
        int screenWidth;
        int screenHeight;
        private BeeSwarm field;
        private float wingAngle = 0; // Current wing deployment angle
        private float headAngle = 0; // Current wing deployment angle
        private int wing1;          // Index to the wing 1 bone
        private int wing2;          // Index to the wing 2 bone
        private int head;          // Index to the wing 2 bone
        private Model bee;
        private bool WingMovingUp = false;
        private bool HeadMovingUp = false;


        /// <summary>
        /// Rate we move the ship per second (as a vector)
        /// </summary>
        private Vector3 moveSpeed = new Vector3(-100, 0, 0);

        public StepGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Don't forget to put this above the Game1 class!

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //FOR FINAL PROJECT
            SpriteTexture = Content.Load<Texture2D>("Splash");
            screenWidth = 800;
            screenHeight = 480;
            ship = Content.Load<Model>("Ship");
            bee = Content.Load<Model>("Bee");
            wing1 = bee.Bones.IndexOf(bee.Bones["RightWing1"]);
            wing2 = bee.Bones.IndexOf(bee.Bones["LeftWing1"]);
            head = bee.Bones.IndexOf(bee.Bones["Head"]);


            //FOR FINAL PROJECT
            gameState = GameState.Splash;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            if (gameState == GameState.Splash)
            {
                if (newState.IsKeyDown(Keys.Space))
                {
                    gameState = GameState.Game;
                }
            }
            else if (gameState == GameState.Game)
            {

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                angle += rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                float wingDeployTime = 0.05f;        // Seconds
                float headDeployTime = 0.2f;        // Seconds

                if (WingMovingUp == true)
                {
                    wingAngle += (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / wingDeployTime);
                    if (wingAngle > 0.20f){
                        wingAngle = 0.20f;
                        WingMovingUp = false;}
                }
                else if (WingMovingUp == false)
                {
                    wingAngle -= (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / wingDeployTime);
                    if (wingAngle < -0.20f){
                        wingAngle = -0.20f;
                        WingMovingUp = true;
                    }
                }

                if (HeadMovingUp == true)
                {
                    headAngle += (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / headDeployTime);
                    if (headAngle > 0.20f)
                    {
                        headAngle = 0.20f;
                        HeadMovingUp = false;
                    }
                }
                else if (HeadMovingUp == false)
                {
                    headAngle -= (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / headDeployTime);
                    if (headAngle < -0.20f)
                    {
                        headAngle = -0.20f;
                        HeadMovingUp = true;
                    }
                }

                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            if (gameState == GameState.Splash)
            {
                spriteBatch.Begin();
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(SpriteTexture, screenRectangle, Color.White);
                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                Matrix bank = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), 0.78f);
                Matrix pitch = Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -0.52f);
                Matrix spin = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
                Matrix move = Matrix.CreateTranslation(position);
                Matrix loop = Matrix.CreateTranslation(0, -1000, 0) *
                    Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -angle) *
                    Matrix.CreateTranslation(position);
                //DrawModel(ship, loop);
                Matrix asteroidPOS1 = Matrix.CreateTranslation(0, 0, 0);
                DrawModel(bee, asteroidPOS1);

                // DrawModel(ship, move * spin);

                // TODO: Add your drawing code here

                base.Draw(gameTime);
            }
        }

        private void DrawModel(Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            transforms[wing1] = Matrix.CreateRotationY(wingAngle) * transforms[wing1];
            transforms[wing2] = Matrix.CreateRotationY(-wingAngle) * transforms[wing2];
            transforms[head] = Matrix.CreateRotationY(headAngle) * transforms[head];

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = Matrix.CreateLookAt(new Vector3(10, 10, 10),
                                                      new Vector3(0, 0, 0),
                                                      new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(35),
                        graphics.GraphicsDevice.Viewport.AspectRatio, 10, 10000);
                }
                mesh.Draw();
            }
        }
    }
}
