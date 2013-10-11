using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    public class Roses
    {
        public class Rose
        {
            public float size;
            public Vector3 position;
            public enum RoseState {isGrowing, isWaitingJoin, isInUse, isWaitingLeave, isDying};
            public RoseState roseState;
        }

        private BugGame game;
        private Model model;

        private Rose originRose;

        private LinkedList<Rose> roseList = new LinkedList<Rose>();
        public  LinkedList<Rose> RoseList { get { return roseList; } }

        private Random random = new Random();

        private Vector3[] Position = new Vector3[36];

        public Roses(BugGame game)
        {
            this.game = game;
        }

        public void InitializeOriginRose()
        {
            originRose = new Rose();
            originRose.size = 2f;
            originRose.position = new Vector3(0, 0, 0);
            originRose.roseState = Rose.RoseState.isInUse;
        }

        public void InitializeRoseList()
        {
            while (roseList.Count < 36)
            {
                Rose rose = new Rose();
                rose.size = .1f;
                rose.position = RandomPosition(random.Next(36));
                //rose.playerCanLandOnFlower = false;

                bool addRose = true;

                foreach (Rose existingRose in roseList)
                {
                    if (rose.position == existingRose.position) addRose = false;
                }

                if (addRose) roseList.AddLast(rose);
            }
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("Rose");
        }

        public void Update(GameTime gameTime)
        {
            foreach (Rose rose in roseList)
            {
                if (rose.size < 2f)
                {
                    rose.size += (.01f);
                }
            }
        }

        public void Draw(GraphicsDeviceManager graphics, GameTime gameTime)
        {
            DrawModel(graphics, model, Matrix.CreateScale(originRose.size) * Matrix.CreateTranslation(originRose.position));

            foreach (Rose rose in roseList)
            {
                DrawModel(graphics, model, Matrix.CreateScale(rose.size) * Matrix.CreateTranslation(rose.position));
            }
        }

        public bool inLandingZone(BoundingSphere sphere)
        {
            for (LinkedListNode<Rose> roseNode = roseList.First; roseNode != null; )
            {
                LinkedListNode<Rose> nextNode = roseNode.Next;
                Rose rose = roseNode.Value;

                BoundingSphere bs = model.Meshes[7].BoundingSphere;
                bs = bs.Transform(model.Bones[7].Transform);

                //bs.Radius *= rose.size;
                bs.Radius *= (rose.size * 2);
                bs.Center += (rose.position + new Vector3(0, 34f, 0));
                //bs.Center += rose.position;

                if (sphere.Intersects(bs))
                {
                    return true;
                }

                roseNode = nextNode;
            }
            return false;
        }

        private void DrawModel(GraphicsDeviceManager graphics, Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

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

        private Vector3 RandomPosition(int position)
        {
            //Row 01
            Position[0]  = new Vector3(-50, 0, 150);
            Position[1]  = new Vector3(  0, 0, 150);
            Position[2]  = new Vector3( 50, 0, 150);
            //Row 02
            Position[3]  = new Vector3(-100, 0, 100);
            Position[4]  = new Vector3( -50, 0, 100);
            Position[5]  = new Vector3(   0, 0, 100);
            Position[6]  = new Vector3(  50, 0, 100);
            Position[7]  = new Vector3( 100, 0, 100);
            //Row 03
            Position[8]  = new Vector3(-150, 0, 50);
            Position[9]  = new Vector3(-100, 0, 50);
            Position[10] = new Vector3( -50, 0, 50);
            Position[11] = new Vector3(   0, 0, 50);
            Position[12] = new Vector3(  50, 0, 50);
            Position[13] = new Vector3( 100, 0, 50);
            Position[14] = new Vector3( 150, 0, 50);
            //Row 04
            Position[15] = new Vector3(-150, 0, 0);
            Position[16] = new Vector3(-100, 0, 0);
            Position[17] = new Vector3( -50, 0, 0);
            //Origin Rose -----------------------
            Position[18] = new Vector3(  50, 0, 0);
            Position[19] = new Vector3( 100, 0, 0);
            Position[20] = new Vector3( 150, 0, 0);
            //Row 05
            Position[21] = new Vector3(-150, 0, -50);
            Position[22] = new Vector3(-100, 0, -50);
            Position[23] = new Vector3( -50, 0, -50);
            Position[24] = new Vector3(   0, 0, -50);
            Position[25] = new Vector3(  50, 0, -50);
            Position[26] = new Vector3( 100, 0, -50);
            Position[27] = new Vector3( 150, 0, -50);
            //Row 06
            Position[28] = new Vector3(-100, 0, -100);
            Position[29] = new Vector3( -50, 0, -100);
            Position[30] = new Vector3(   0, 0, -100);
            Position[31] = new Vector3(  50, 0, -100);
            Position[32] = new Vector3( 100, 0, -100);
            //Row 07
            Position[33] = new Vector3(-50, 0, -150);
            Position[34] = new Vector3(  0, 0, -150);
            Position[35] = new Vector3( 50, 0, -150);

            return Position[position];
        }
    }
}

