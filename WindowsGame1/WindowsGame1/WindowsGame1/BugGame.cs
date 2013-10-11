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

namespace WindowsGame1
{
    /// <summary>
    /// /// This is the main type for your game
    /// </summary>
    public class BugGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private SpriteFont scoreFont;
        private Camera camera;
        private Model field;
        private Model sky;
        private Vector3 skyPosition = new Vector3(0, -5, 0);
        private Butterfly butterfly;
        private Roses rose;

        public enum GameState { Splash, Game, End }
        GameState gameState;
        private Texture2D Splash;
        private Texture2D End;
        private Texture2D Heart;
        int screenWidth;
        int screenHeight;
        private BeeSwarm swarm;
        private int score;
        private int lives;
        private int livecounter =0 ;

        private KeyboardState lastKeyboardState;

        /// <summary>
        /// A reference to the audio engine we use
        /// </summary>
        AudioEngine audioEngine;

        /// <summary>
        /// The loaded audio wave bank
        /// </summary>
        WaveBank waveBank;

        /// <summary>
        /// The loaded audio sound bank
        /// </summary>
        SoundBank soundBank;

        //private Model butterfly;

        public BugGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            butterfly = new Butterfly(this);
            swarm = new BeeSwarm(this);
            rose = new Roses(this);
            camera = new Camera(graphics);
            score = 0;
            lives = 3;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera.Initialize();
            base.Initialize();
            rose.InitializeOriginRose();
            rose.InitializeRoseList();
            lastKeyboardState = Keyboard.GetState();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Splash = Content.Load<Texture2D>("Splash");
            End = Content.Load<Texture2D>("Score");
            Heart = Content.Load<Texture2D>("Heart");
            screenWidth = 800;
            screenHeight = 480;

            swarm.LoadContent(Content);
            butterfly.LoadContent(Content);
            rose.LoadContent(Content);
            field = Content.Load<Model>("GrassField2");
            sky = Content.Load<Model>("Sky");
            scoreFont = Content.Load<SpriteFont>("scorefont");

            audioEngine = new AudioEngine("Content\\WindowsGameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");

            // TODO: use this.Content to load your game content here
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
            KeyboardState keyboardState = Keyboard.GetState();
            if (gameState == GameState.Splash)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    gameState = GameState.Game;
                }
            }
            else if (gameState == GameState.Game)
            {
                camera.Eye = Vector3.Transform(new Vector3(0, 10, -50), butterfly.Transform);
                camera.Center = butterfly.Position;
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                

                // TODO: Add your update logic here

                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    butterfly.IncreaseAltitude = true;
                }
                else
                    butterfly.IncreaseAltitude = false;

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    butterfly.TurnRate = 1;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    butterfly.TurnRate = -1;
                }
                else
                {
                    butterfly.TurnRate = 0;
                }

                if (keyboardState.IsKeyDown(Keys.W))
                {
                    butterfly.Move = 1;
                }
                else
                {
                    butterfly.Move = 0;
                }

                lastKeyboardState = keyboardState;


                butterfly.Update(gameTime);
                swarm.Update(gameTime, butterfly.Position);
                rose.Update(gameTime);
                camera.Update(gameTime);
                base.Update(gameTime);

                if (livecounter >= 0)
                    livecounter--;

                Matrix[] transforms = new Matrix[butterfly.Model.Bones.Count];
                butterfly.Model.CopyAbsoluteBoneTransformsTo(transforms);
                Matrix buttTransform = butterfly.Transform;

                foreach (ModelMesh mesh in butterfly.Model.Meshes)
                {
                    BoundingSphere bs = mesh.BoundingSphere;
                    bs = bs.Transform(transforms[mesh.ParentBone.Index] * buttTransform);
                    if (rose.inLandingZone(bs))
                    {
                        //butterfly.Position = Vector3.Zero;
                        //butterfly.WingAngle = 0;
                        score = score + 1;
                    }
                    if (swarm.TestSphereForCollision(bs))
                    {
                        butterfly.Position = Vector3.Zero;
                        swarm.Restart();
                        
                        //butterfly.WingAngle = 0;
                        if(livecounter<= 0)
                            lives = lives - 1;
                        livecounter = 50;
                    }
                }


                //current test function
                if (lives <= 0)
                {
               
                    gameState = GameState.End;
                }
                audioEngine.Update();
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
                spriteBatch.Draw(Splash, screenRectangle, Color.White);
                spriteBatch.End();
            }
            else if(gameState == GameState.Game)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                Matrix position = Matrix.CreateTranslation(new Vector3(0, 0, 0));
                Matrix scale = Matrix.CreateScale(20f, 0, 20f);

                DrawGround(graphics, field, scale * position);
                Matrix transform = Matrix.CreateTranslation(skyPosition);

                DrawSky(graphics, sky, transform);
                // TODO: Add your drawing code here
                swarm.Draw(graphics, gameTime);
                rose.Draw(graphics, gameTime);
                butterfly.Draw(graphics, gameTime);
                base.Draw(gameTime);

                spriteBatch.Begin();
                string scoreString = String.Format("{0:0000}", score);
                spriteBatch.DrawString(scoreFont, scoreString, new Vector2(10, 10), Color.White);
                if (lives > 0)
                {
                    spriteBatch.Draw(Heart, new Vector2(720, 10), Color.White);
                }
                if (lives > 1)
                {
                    spriteBatch.Draw(Heart, new Vector2(680, 10), Color.White);
                }
                if (lives > 2)
                {
                    spriteBatch.Draw(Heart, new Vector2(640, 10), Color.White);
                }
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(End, screenRectangle, Color.White);
                string scoreString = String.Format("{0:0000}", score);
                spriteBatch.DrawString(scoreFont, scoreString, new Vector2(380, 240), Color.White);
                spriteBatch.End();
            }
        }

        private void DrawGround(GraphicsDeviceManager graphics, Model model, Matrix world)
        {
           Matrix[] transforms = new Matrix[model.Bones.Count];
           model.CopyAbsoluteBoneTransformsTo(transforms);
           
           //transforms[wing1] = Matrix.CreateRotationZ(wingAngle) * transforms[wing1];
          // transforms[wing2] = Matrix.CreateRotationZ(-wingAngle) * transforms[wing2];
           //Console.WriteLine("here");
           foreach (ModelMesh mesh in model.Meshes)
           {
               foreach (BasicEffect effect in mesh.Effects)
               {
                   effect.EnableDefaultLighting();
                   effect.World = transforms[mesh.ParentBone.Index] * world;
                   effect.View = Camera.View;
                   effect.Projection = Camera.Projection;
               }
               mesh.Draw();
           }
        }

        private void DrawSky(GraphicsDeviceManager graphics, Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            //transforms[wing1] = Matrix.CreateRotationZ(wingAngle) * transforms[wing1];
            // transforms[wing2] = Matrix.CreateRotationZ(-wingAngle) * transforms[wing2];
            //Console.WriteLine("here");
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                }
                mesh.Draw();
            }
        }

        public Camera Camera { get { return camera; } }
        public SoundBank SoundBank { get { return soundBank; } }

    }
}
