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
   public class Butterfly
    {
       private enum WingStates { Up, Down, NotMoving };
       private enum PollenatingStates { Up, Down, FlyAway, Nothing };
       WingStates states;
       PollenatingStates pollenStates;
       private BugGame game;
       bool increaseAltitude = false;
       private Model model;
       public Model Model { get { return model; } }
       private float gravity = 9.81f;
       Vector3 position = Vector3.Zero;
       public Vector3 Position { get { return position; } set { position = value; } }
       
       private bool isPolenating = false;

       private int wing1;          // Index to the wing 1 bone
       private int wing2;          // Index to the wing 2 bone
       private float wingAngle = 0; // Current wing deployment angle

       private bool deployed = false;
       private float YSpeed = 10;
       private float movementSpeed = 10;
       private float move = 0;

       private float azimuth = 0;
       private float turnRate = 0;

       float delta = 0;
       private float rotationSpeed = 3.14f;

       public Butterfly(BugGame game)
       {
           this.game = game;
           states = WingStates.Up;
       }

       /// <summary>
       /// This function is called to load content into this component
       /// of our game.
       /// </summary>
       /// <param name="content">The content manager to load from.</param>
       public void LoadContent(ContentManager content)
       {
           model = content.Load<Model>("Butterfly");
           wing1 = model.Bones.IndexOf(model.Bones["WingLeft"]);
           wing2 = model.Bones.IndexOf(model.Bones["WingRight"]);
       }

       /// <summary>
       /// This function is called to update this component of our game
       /// to the current game time.
       /// </summary>
       /// <param name="gameTime"></param>
       public void Update(GameTime gameTime)
       {

           azimuth += turnRate * rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
           Matrix transform = Matrix.CreateRotationY(azimuth);
           Vector3 direction = Vector3.TransformNormal(new Vector3(0, 0, 1), transform);
           
           delta += (float)gameTime.ElapsedGameTime.TotalSeconds;
           switch (pollenStates)
           {
               case PollenatingStates.Up:
                   position += new Vector3(0, 1, 0) * (YSpeed+20) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                   if (delta > 1.0f)
                   {
                       pollenStates = PollenatingStates.Down;
                       delta = 0;
                       return;
                   }
                   break;
               case PollenatingStates.Down:
                   position -= new Vector3(0, 1, 0) * YSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                   if (delta > 1.0f)
                   {
                       pollenStates = PollenatingStates.FlyAway;
                       delta = 0;
                       return;
                   }
                   break;
               case PollenatingStates.FlyAway:
                   position += 1 * direction * movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                   if (delta > 1.0f)
                   {
                       delta = 0;
                       pollenStates = PollenatingStates.Nothing;
                       isPolenating = false;
                       return;
                   }
                   break;
           }
           if(!isPolenating)
           {
             if (increaseAltitude)
            {
                 //states = WingStates.Up;
                position += new Vector3(0, 1, 0) * YSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (position.Y > 2000)
                    position.Y = 2000;
               
              }
             else
             {
                  position -= new Vector3(0, 1, 0) * gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                  if (position.Y < 0)
                   {
                      position.Y = 0;
                      // states = WingStates.NotMoving;
                       //wingAngle = 0;
                  }
             }


            position += move * direction * movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
           }
           float wingMoveTime = .50f;
           switch (states)
           {
               
               case WingStates.Up:
                   {
                       wingAngle += .3f * (float)gameTime.ElapsedGameTime.TotalSeconds / wingMoveTime;
                       if (wingAngle > .3f)
                           states = WingStates.Down;

                       break;
                   }
               case WingStates.Down:
                  {
                      wingAngle -= .3f * (float)gameTime.ElapsedGameTime.TotalSeconds / wingMoveTime;
                      if (wingAngle < 0)
                          states = WingStates.Up;
                      break;
                  }
           }
           

       }

       /// <summary>
       /// This function is called to draw this game component.
       /// </summary>
       /// <param name="graphics"></param>
       /// <param name="gameTime"></param>
       public void Draw(GraphicsDeviceManager graphics, GameTime gameTime)
       {
           Matrix translation = Matrix.CreateTranslation(position);
           Matrix turning = Matrix.CreateRotationY(azimuth);
           DrawModel(graphics, model,turning *translation );
       }


       private void DrawModel(GraphicsDeviceManager graphics, Model model, Matrix world)
       {

           Matrix[] transforms = new Matrix[model.Bones.Count];
           model.CopyAbsoluteBoneTransformsTo(transforms);
          // transforms[wing1] = Matrix.CreateRotationZ(wingAngle) * transforms[wing1];
           //transforms[wing2] = Matrix.CreateRotationZ(-wingAngle) * transforms[wing2];

           int count = model.Bones.Count;
           for (int i = 0; i < count; i++)
           {
               if (i == wing1)
                   transforms[i] = Matrix.CreateRotationY(wingAngle);
               else if (i == wing2)
                   transforms[i] = Matrix.CreateRotationY(-wingAngle);
               else
                   transforms[i] = Matrix.Identity;

               ModelBone bone = model.Bones[i];
               if (bone.Parent == null)
               {
                   transforms[i] *= bone.Transform;
               }
               else
               {
                   transforms[i] *= bone.Transform * transforms[bone.Parent.Index];
               }
           }


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

       public void SetPollenatingStateToPollenate()
       {
           pollenStates = PollenatingStates.Up;
       }

       public bool Deployed { get { return deployed; } set { deployed = value; } }
       public bool IncreaseAltitude { get { return increaseAltitude; } set { increaseAltitude = value; } }
       public float TurnRate {get {return turnRate;}  set { turnRate = value;} }
       public float Move { get { return move; } set { move = value; } }
       public bool IsPollenating { get { return isPolenating; } set { isPolenating = value; } }

       public Matrix Transform { get { Matrix transform = Matrix.CreateRotationY(azimuth) * Matrix.CreateTranslation(position); return transform; } }
    }
}
