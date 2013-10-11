using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{    
   public class Camera
   {
       private GraphicsDeviceManager graphics;
       private Vector3 eye = new Vector3(30, 30, 30);
       private Vector3 center = new Vector3(0, 0, 0);
       private Vector3 up = new Vector3(0, 1, 0);
       private float fov = MathHelper.ToRadians(35);
       private float znear = 10;
       private float zfar = 10000;

       private bool mousePitchYaw = true;
       private bool mousePanTilt = true;

       private MouseState lastMouseState;

       private Matrix view;
       private Matrix projection;

       public void Initialize()
       {
           ComputeView();
           ComputeProjection();
           lastMouseState = Mouse.GetState();
       }

       public void Update(GameTime gameTime)
       {
           MouseState mouseState = Mouse.GetState();

           if (mousePitchYaw && mouseState.LeftButton == ButtonState.Pressed &&
               lastMouseState.LeftButton == ButtonState.Pressed)
           {
               float changeY = mouseState.Y - lastMouseState.Y;
               Pitch(-changeY * 0.005f);

               float changeX = mouseState.X - lastMouseState.X;
               Yaw(changeX * 0.005f);
           }

           lastMouseState = mouseState;
       }

       public Camera(GraphicsDeviceManager graphics)
       {
           this.graphics = graphics;
       }

       private void ComputeView()
       {
           view = Matrix.CreateLookAt(eye, center, up);
       }

       private void ComputeProjection()
       {
           projection = Matrix.CreatePerspectiveFieldOfView(fov,
               graphics.GraphicsDevice.Viewport.AspectRatio, znear, zfar);
       }

       public void Pitch(float angle)
       {
           // Need a vector in the camera X direction
           Vector3 cameraZ = eye - center;
           Vector3 cameraX = Vector3.Cross(up, cameraZ);
           float len = cameraX.LengthSquared();
           if (len > 0)
               cameraX.Normalize();
           else
               cameraX = new Vector3(1, 0, 0);

           Matrix t1 = Matrix.CreateTranslation(-center);
           Matrix r = Matrix.CreateFromAxisAngle(cameraX, angle);
           Matrix t2 = Matrix.CreateTranslation(center);

           Matrix M = t1 * r * t2;
           eye = Vector3.Transform(eye, M);
           ComputeView();
       }
       public void Yaw(float angle)
       {
           // Need a vector in the camera X direction
           Vector3 cameraZ = eye - center;
           Vector3 cameraX = Vector3.Cross(up, cameraZ);
           Vector3 cameraY = Vector3.Cross(cameraZ, cameraX);
           float len = cameraY.LengthSquared();
           if (len > 0)
               cameraY.Normalize();
           else
               cameraY = new Vector3(0, 1, 0);

           Matrix t1 = Matrix.CreateTranslation(-center);
           Matrix r = Matrix.CreateFromAxisAngle(cameraY, angle);
           Matrix t2 = Matrix.CreateTranslation(center);

           Matrix M = t1 * r * t2;
           eye = Vector3.Transform(eye, M);
           ComputeView();
       }

       public Matrix View { get { return view; } }
       public Matrix Projection { get { return projection; } }
       public bool MousePitchYaw { get { return mousePitchYaw; } set { mousePitchYaw = value; } }
       public bool MousePanTilt { get { return mousePanTilt; } set { mousePanTilt = value; } }

       public Vector3 Eye { get { return eye; } set { eye = value; ComputeView(); } }
       public Vector3 Center { get { return center; } set { center = value; ComputeView(); } }


   }
}
