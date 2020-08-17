using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace LearnOpenGL
{
    public class OpenTK_Window : GameWindow
    {
        ResourceTracker tracker = new ResourceTracker();

        private float time;

        #region Input handling

        /// <summary>
        /// True if the mouse is captured.
        /// </summary>
        private bool mouseIsCaptured = false;

        /// <summary>
        /// True if the mouse is moving within the viewport.
        /// </summary>
        private bool mouseInViewport = false;

        /// <summary>
        /// True if this is the first mouse move within the viewport.
        /// </summary>
        private bool firstMouseMove = true;

        /// <summary>
        /// Last position of the mouse within the viewport.
        /// </summary>
        private Vector2 lastMousePosition;

        #endregion

        public OpenTK_Window(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            // Cursor starts as visible because we're not yet capturing the mouse.
            this.CursorVisible = true;

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(
                target: BufferTarget.ArrayBuffer,
                buffer: 0);

            GL.BindVertexArray(array: 0);

            GL.UseProgram(program: 0);

            this.tracker.Dispose();

            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.time += (float)e.Time;

            // clear frame; should always be called first.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // double-buffer ftw
            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // const float cameraSpeed = 1.5f;
            // const float sensitivity = 0.2f;

            if (this.Focused)
            {
                KeyboardState input = Keyboard.GetState();

                if (input.IsKeyDown(Key.W))
                {
                    // this.camera.Position += this.camera.Front * cameraSpeed * (float)e.Time; // Forward
                }

                if (input.IsKeyDown(Key.S))
                {
                    // this.camera.Position -= this.camera.Front * cameraSpeed * (float)e.Time; // Backwards
                }
                if (input.IsKeyDown(Key.A))
                {
                    // this.camera.Position -= this.camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Key.D))
                {
                    // this.camera.Position += this.camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Key.Space))
                {
                    // this.camera.Position += this.camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Key.LShift))
                {
                    // this.camera.Position -= this.camera.Up * cameraSpeed * (float)e.Time; // Down
                }
            }

            if (this.mouseInViewport && this.mouseIsCaptured)
            {
                var mouse = Mouse.GetState();

                if (this.firstMouseMove) // this bool variable is initially set to true
                {
                    this.lastMousePosition = new Vector2(mouse.X, mouse.Y);
                    this.firstMouseMove = false;
                }
                else
                {
                    // Calculate the offset of the mouse position
                    var deltaX = mouse.X - this.lastMousePosition.X;
                    var deltaY = mouse.Y - this.lastMousePosition.Y;
                    this.lastMousePosition = new Vector2(mouse.X, mouse.Y);

                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    // this.camera.Yaw += deltaX * sensitivity;
                    // this.camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top=
                }
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Exit();
            }

            base.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Console.WriteLine($"OnMouseDown: Button = {e.Button}, IsPressed = {e.IsPressed}");

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Console.WriteLine($"OnMouseUp: Button = {e.Button}, IsPressed = {e.IsPressed}");

            if (e.Button == MouseButton.Left)
            {
                this.mouseIsCaptured = !this.mouseIsCaptured;
                this.CursorVisible = !this.mouseIsCaptured;
                if (!this.mouseIsCaptured)
                {
                    this.firstMouseMove = true;
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Console.WriteLine($"OnMouseMove: ");

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Console.WriteLine($"OnMouseWheel: Delta = {e.Delta}, DeltaPrecise = {e.DeltaPrecise}");

            // this.camera.FOV -= e.DeltaPrecise;

            base.OnMouseWheel(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.mouseInViewport = true;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouseInViewport = false;
            this.firstMouseMove = true;

            base.OnMouseLeave(e);
        }
    }
}
