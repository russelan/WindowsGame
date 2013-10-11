using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1
{

    public class BeeSwarm
    {
        private BugGame game;
        private Model model;
        private Random random = new Random();

        private int wing1;          // Index to the wing 1 bone
        private int wing2;          // Index to the wing 2 bone
        private int head;          // Index to the head bone

        private LinkedList<Bee> bees = new LinkedList<Bee>();

        public LinkedList<Bee> Bees { get { return bees; } }

        public BeeSwarm(BugGame game)
        {
            this.game = game;
        }

        public class Bee
        {
            public int model;
            public int noise;
            public Vector3 position;
            public Vector3 velocity;
            public float size;
            public bool WingMovingUp = false;
            public bool HeadMovingUp = false;
            public float wingAngle = 0; // Current wing deployment angle
            public float headAngle = 0; // Current wing deployment angle
        }

        /// <summary>
        /// This function is called to load content into this component
        /// of our game.
        /// </summary>
        /// <param name="content">The content manager to load from.</param>
        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("Bee");
            wing1 = model.Bones.IndexOf(model.Bones["RightWing1"]);
            wing2 = model.Bones.IndexOf(model.Bones["LeftWing1"]);
            head = model.Bones.IndexOf(model.Bones["Head"]);

            BuildField(10.0f);
        }

        public void BuildField(float size)
        {
            while (Bees.Count < 50)
            {
                Bee bee = new Bee();
                bee.model = random.Next(4);
                bee.position = RandomVector(-100, 100);
                bee.velocity = RandomVector(-.1f, .1f);
                bee.size = 3;
                bee.noise = 0;

                if (((bee.position.X < 10 && bee.position.X > -10) || (bee.position.Y < 10 ) || (bee.position.Z < 10 && bee.position.Z > -10)))
                    continue;

                Bees.AddLast(bee);
            }
        }

        public void Restart()
        {
            Bees.Clear();
            BuildField(10.0f);
        }

        /// <summary>
        /// This function is called to update this component of our game
        /// to the current game time.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, Vector3 butpos)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

           

            for (LinkedListNode<Bee> blastNode = Bees.First; blastNode != null; )
            {


                // blastNode is the current node and we set
                // nextBlast to the next node in the list
                LinkedListNode<Bee> nextBlast = blastNode.Next;

                // This is the actual blast object we are working on 
                Bee ast = blastNode.Value;

                float result = 1;
                result = Vector3.Distance(butpos, ast.position);

                if (result > 20)
                {
                    if ((ast.position.X > 130 || ast.position.X < -130) || (ast.position.Y < 0 || ast.position.Y > 130) || (ast.position.Y > 130 || ast.position.Y < -130))
                        ast.velocity = RandomVector(-.1f, .1f);
                    // Update the position
                    ast.position += ast.velocity;
                }
                else
                {
                    if (ast.noise <= 0)
                    {
                        game.SoundBank.PlayCue("Bee");
                        ast.noise = 100;

                    }
                    if (ast.noise >= 0)
                    {
                        ast.noise--;
                    }

                    if (ast.position.X - butpos.X < 0)
                        ast.position.X += .05f;
                    else
                        ast.position.X -= .05f;

                    if (ast.position.Y - butpos.Y < 0)
                        ast.position.Y += .05f;
                    else
                        ast.position.Y -= .05f;

                    if (ast.position.Z - butpos.Z < 0)
                        ast.position.Z += .05f;
                    else
                        ast.position.Z -= .05f;
                            

                }

                float wingDeployTime = 0.05f;        // Seconds
                float headDeployTime = 0.2f;        // Seconds

                if (ast.WingMovingUp == true)
                {
                    ast.wingAngle += (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / wingDeployTime);
                    if (ast.wingAngle > 0.20f)
                    {
                        ast.wingAngle = 0.20f;
                        ast.WingMovingUp = false;
                    }
                }
                else if (ast.WingMovingUp == false)
                {
                    ast.wingAngle -= (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / wingDeployTime);
                    if (ast.wingAngle < -0.20f)
                    {
                        ast.wingAngle = -0.20f;
                        ast.WingMovingUp = true;
                    }
                }

                if (ast.HeadMovingUp == true)
                {
                    ast.headAngle += (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / headDeployTime);
                    if (ast.headAngle > 0.20f)
                    {
                        ast.headAngle = 0.20f;
                        ast.HeadMovingUp = false;
                    }
                }
                else if (ast.HeadMovingUp == false)
                {
                    ast.headAngle -= (float)(0.20 * gameTime.ElapsedGameTime.TotalSeconds / headDeployTime);
                    if (ast.headAngle < -0.20f)
                    {
                        ast.headAngle = -0.20f;
                        ast.HeadMovingUp = true;
                    }
                }

                blastNode = nextBlast;
            }

        }

        private Vector3 RandomVector(float min, float max)
        {
            return new Vector3((float)(min + (random.NextDouble() * (max - min))),
                (float)(min + (random.NextDouble() * (max - min))),
                (float)(min + (random.NextDouble() * (max - min))));
        }

        /// <summary>
        /// Tests a laser point to see if it is in the bounding sphere of any of 
        /// our Bees.  If so, it deletes Bee and
        /// returns true.
        /// </summary>
        /// <param name="position">Tip of the laser</param>
        /// <returns></returns>
        public bool TestSphereForCollision(BoundingSphere sphere)
        {
            for (LinkedListNode<Bee> BeeNode = Bees.First; BeeNode != null; )
            {
                LinkedListNode<Bee> nextNode = BeeNode.Next;
                Bee bee = BeeNode.Value;

                // Obtain a bounding sphere for the Bee.  I can get away
                // with this here because I know the model has exactly one mesh
                // and exactly one bone.
                BoundingSphere bs = model.Meshes[0].BoundingSphere;
                bs = bs.Transform(model.Bones[0].Transform);

                // Move this to world coordinates.  Note how easy it is to 
                // transform a bounding sphere
                bs.Radius *= bee.size;
                bs.Center += bee.position;

                if (sphere.Intersects(bs))
                {
                    return true;
                }

                BeeNode = nextNode;
            }

            return false;
        }

        /// <summary>
        /// This function is called to draw this game component.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gameTime"></param>
        public void Draw(GraphicsDeviceManager graphics, GameTime gameTime)
        {
            foreach (Bee ast in Bees)
            {
                DrawModel(graphics, model, Matrix.CreateScale(ast.size) * Matrix.CreateTranslation(ast.position), ast);
            }
        }


        private void DrawModel(GraphicsDeviceManager graphics, Model model, Matrix world, Bee ast)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            transforms[wing1] = Matrix.CreateRotationY(ast.wingAngle) * transforms[wing1];
            transforms[wing2] = Matrix.CreateRotationY(-ast.wingAngle) * transforms[wing2];
            transforms[head] = Matrix.CreateRotationY(ast.headAngle) * transforms[head];


            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;      
                    effect.View = game.Camera.View;
                    effect.Projection = game.Camera.Projection;
                }
                mesh.Draw();
            }
        }
    }

}
