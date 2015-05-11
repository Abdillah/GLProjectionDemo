using System;
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
            using (var game = new GameWindow())
            {
                game.Width = 800;
                game.Height = 600;

                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                };
 
                game.Resize += (sender, e) =>
                {
                    //Setting up a viewport using the native windows Width and Height
                    //you can have multiple viewports for splitscreen but for now just one.
                    GL.Viewport(0, 0, game.Width, game.Height);

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

                    //Then we tell GL to use are matrix as the new Projection matrix.
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadMatrix(ref perspective_matrix);

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
                    GL.Translate(-2f, -1f, -5);

                    //Draw
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Red);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Vertex3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(1.0f, 1.0f, 0.0f);
                    GL.Vertex3(0.0f, 1.0f, 0.0f);
                    GL.End();
 
                    game.SwapBuffers();

                    return;
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Red);
                    GL.Vertex3(20.0f, 20.0f, 0.0f);
                    GL.Vertex3(400.0f, 20.0f, 0.0f);
                    GL.Vertex3(400.0f, 400.0f, 0.0f);
                    GL.Vertex3(20.0f, 400.0f, 0.0f);
                    GL.End();

                    GL.Begin(PrimitiveType.Quads);
                    GL.Color3(Color.Blue);
                    GL.Vertex3(20.0f, 20.0f, 10.0f);
                    GL.Vertex3(400.0f, 20.0f, 10.0f);
                    GL.Vertex3(400.0f, 300.0f, 10.0f);
                    GL.Vertex3(20.0f, 400.0f, 10.0f);
                    GL.End();
                };
 
                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}
