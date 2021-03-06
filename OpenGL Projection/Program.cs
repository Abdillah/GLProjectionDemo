﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;

namespace OpenGL_Projection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //variabel Wall yang bertipe array 3D float
            float[][] Wall = {
                           // Front
                           new float[] {0.0f, 0.0f, 0.0f},
						   new float[] {2.0f, 0.0f, 0.0f},
						   new float[] {2.0f, 2.0f, 0.0f},
						   new float[] {0.0f, 2.0f, 0.0f},
                           // Rear
						   new float[] {0.0f, 0.0f, 2.0f},
						   new float[] {2.0f, 0.0f, 2.0f},
						   new float[] {2.0f, 2.0f, 2.0f},
						   new float[] {0.0f, 2.0f, 2.0f},};

           
            float[][] Roof = {
                              new float []{ 1.0f, 3.5f,  1.0f}, // Peak
                              new float []{-0.5f, 2.0f,  2.5f},
                              new float []{ 2.5f, 2.0f,  2.5f},
                              new float []{ 2.5f, 2.0f, -0.5f},
                              new float []{-0.5f, 2.0f, -0.5f}};

            int horizontalRotationAngle = 0;
            int verticalRotationAngle = 0;
            Vector3d horizontalNavigationVector = new Vector3d(0, 1, 0);
            Vector3d verticalNavigationVector = new Vector3d(1, 0, 0);
            bool isOrthographic = true;

            using (var game = new GameWindow())
            {
                game.Width = 800;
                game.Height = 600;

                //Get the aspect ratio of the screen
                double aspect_ratio = game.Width / (double)game.Height;
                //Field of view of are camera
                float fov = 1.0f;
                //The nearest the camera can see, want to keep this number >= 0.1f else visible clipping ensues
                float near_distance = 1.0f;
                //The farthest the camera can see, depending on how far you want to draw this can be up to float.MaxValue
                float far_distance = 1000.0f;

                //Now we pass the parameters onto are matrix
                OpenTK.Matrix4 perspective_matrix =
                   OpenTK.Matrix4.CreatePerspectiveFieldOfView(fov, (float)aspect_ratio, near_distance, far_distance);

                OpenTK.Matrix4 orthographic_matrix =
                   OpenTK.Matrix4.CreateOrthographic(16, 12, near_distance, far_distance);
                    


                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                };

                game.KeyUp += (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        isOrthographic = isOrthographic ? false : true;

                        //Then we tell GL to use are matrix as the new Projection matrix.
                        GL.MatrixMode(MatrixMode.Projection);

                        if (isOrthographic)
                        {
                            GL.LoadMatrix(ref orthographic_matrix);
                        }
                        else
                        {
                            GL.LoadMatrix(ref perspective_matrix);
                        }
                    }
                };
 
                game.Resize += (sender, e) =>
                {
                    //Setting up a viewport using the native windows Width and Height
                    //you can have multiple viewports for splitscreen but for now just one.
                    GL.Viewport(0, 0, game.Width, game.Height);

                    //Then we tell GL to use are matrix as the new Projection matrix.
                    GL.MatrixMode(MatrixMode.Projection);

                    if (isOrthographic)
                    {
                        GL.LoadMatrix(ref orthographic_matrix);
                    }
                    else
                    {
                        GL.LoadMatrix(ref perspective_matrix);
                    }
					
                    Matrix4 lookat = Matrix4.LookAt(0, 0, 0, 0, 0, 0, 0, 1, 0);

                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadMatrix(ref lookat);
                };
 
                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                    if (game.Keyboard[Key.Left])
                    {
                        horizontalRotationAngle--;
                        if (horizontalRotationAngle < 0)
                        {
                            horizontalRotationAngle = 360;
                        }
                    }
                    if (game.Keyboard[Key.Right])
                    {
                        horizontalRotationAngle++;
                        if (horizontalRotationAngle > 360)
                        {
                            horizontalRotationAngle = 0;
                        }
                    }
                    if (game.Keyboard[Key.Up])
                    {
                        verticalRotationAngle--;
                        if (verticalRotationAngle < 0)
                        {
                            verticalRotationAngle = 360;
                        }
                    }
                    if (game.Keyboard[Key.Down])
                    {
                        verticalRotationAngle++;
                        if (verticalRotationAngle > 360)
                        {
                            verticalRotationAngle = 0;
                        }
                    }
                    if (game.Keyboard[Key.Enter])
                    {
                    }
                };
 
                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    // GL.Ortho(0, 800, 0, 600, 0, 500); 
                    //Swap to the modelview so we can draw all of are objects
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();

                    //Now we draw something fancy... a quad
                    //before that we have to set the coords they are currently (0, 0, 0) and are object
                    //will not be visible to us.

                    GL.Translate(-1f, -1f, -15);

                    GL.Rotate(horizontalRotationAngle, horizontalNavigationVector);
                    GL.Rotate(verticalRotationAngle, verticalNavigationVector);

                    //Draw Roof
                    GL.Begin(PrimitiveType.Triangles);
                    GL.Color3(Color.Yellow);
                    GL.Vertex3(Roof[0]);
                    GL.Vertex3(Roof[1]);
                    GL.Vertex3(Roof[4]);
                    GL.End();

                    GL.Begin(PrimitiveType.Triangles);
                    GL.Color3(Color.Blue);
                    GL.Vertex3(Roof[0]);
                    GL.Vertex3(Roof[3]);
                    GL.Vertex3(Roof[4]);
                    GL.End();

                    // Wall part
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Aqua);
                    GL.Vertex3(Wall[0]);
                    GL.Vertex3(Wall[1]);
                    GL.Vertex3(Wall[2]);
                    GL.Vertex3(Wall[3]);
                    GL.End();
									
					GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Green);
                    GL.Vertex3(Wall[4]);
                    GL.Vertex3(Wall[5]);
                    GL.Vertex3(Wall[1]);
                    GL.Vertex3(Wall[0]);
                    GL.End();
					
					GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Blue);
                    GL.Vertex3(Wall[5]);
                    GL.Vertex3(Wall[1]);
                    GL.Vertex3(Wall[2]);
                    GL.Vertex3(Wall[6]);
                    GL.End();
					
					GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Red);
                    GL.Vertex3(Wall[4]);
                    GL.Vertex3(Wall[0]);
                    GL.Vertex3(Wall[3]);
                    GL.Vertex3(Wall[7]);
                    GL.End();

                    GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Yellow);
                    GL.Vertex3(Wall[4]);
                    GL.Vertex3(Wall[5]);
                    GL.Vertex3(Wall[6]);
                    GL.Vertex3(Wall[7]);
                    GL.End();

                    // Roof overdraw
                    GL.Begin(PrimitiveType.Triangles);
                    GL.Color3(Color.Red);
                    GL.Vertex3(Roof[0]);
                    GL.Vertex3(Roof[2]);
                    GL.Vertex3(Roof[1]);
                    GL.End();

                    GL.Begin(PrimitiveType.Triangles);
                    GL.Color3(Color.White);
                    GL.Vertex3(Roof[0]);
                    GL.Vertex3(Roof[2]);
                    GL.Vertex3(Roof[3]);
                    GL.End();

					game.SwapBuffers();

                    return;
                };
 
                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}
