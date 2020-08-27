using img3;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Runtime.CompilerServices;

namespace LearnOpenGL
{
    public class Window : GameWindow
    {
        private ResourceTracker tracker = new ResourceTracker();

        private float[] screenQuadVertices =
        {
            // positions    // texCoords
            -1.0f,  1.0f,   0.0f, 1.0f,
            -1.0f, -1.0f,   0.0f, 0.0f,
             1.0f, -1.0f,   1.0f, 0.0f,

            -1.0f,  1.0f,   0.0f, 1.0f,
             1.0f, -1.0f,   1.0f, 0.0f,
             1.0f,  1.0f,   1.0f, 1.0f
        };
        private VertexBufferObject screenQuadVBO;
        private VertexArrayObject screenQuadVAO;

        private float[] surfaceQuadVertices =
        {
            // positions    // texCoords
            -1.0f,  1.0f,   0.0f, 1.0f,
            -1.0f, -1.0f,   0.0f, 0.0f,
             1.0f, -1.0f,   1.0f, 0.0f,

            -1.0f,  1.0f,   0.0f, 1.0f,
             1.0f, -1.0f,   1.0f, 0.0f,
             1.0f,  1.0f,   1.0f, 1.0f
        };
        private VertexBufferObject surfaceVBO;
        private VertexArrayObject surfaceVAO;

        private Texture textureFramebuffer0;
        private Texture textureFramebuffer1;

        private FramebufferObject framebufferObject0;
        private FramebufferObject framebufferObject1;

        private ShaderProgram shaderProgram;
        private ShaderProgram screenShader;

        private Vector4 brush;

        private bool mouseIsDragging = false;
        private Vector2 lastMousePosition;
        private Vector2 currentMousePosition;

        private float time;
        private float lastFpsTime;
        private int frameCounter;

        private float feed = 0.5f;
        private float kill = 0.5f;

        private ParameterSpace feedKillWarp = new ParameterSpace();

        private float feedMagnitude = 0.002f;
        private float feedIncrement = 0.01f;

        private float killMagnitude = 0.002f;
        private float killIncrement = 0.01f;

        private Vector2 diffusion = new Vector2(1.0f, 0.5f);
        private float diffusionIncrement = 0.001f;

        private float timeMultiplier = 1.0f;
        private float timeMultiplierIncrement = 0.1f;

        private float maxObservedDeltaTime = 0.0f;
        private float maxAllowedDeltaTime = 0.0002f;

        private int renderIterationCount = 50;
        private int renderIterationCountIncrement = 0;

        private float brushWipeCanvas = 1.0f;
        private float brushPaintDot = 2.0f;
        private float brushPaintNoise = 3.0f;
        private int laplacian = 3;

        private float walkSegmentDurationSeconds = 10.0f;
        private float walkDurationRemaining = 0.0f;
        private float walkFeedIncrementPerSecond;
        private float walkKillIncrmentPerSecond;
        private float walkTargetFeed;
        private float walkTargetKill;
        private bool isWalking = false;

        private Random random = new Random();


        public Window(
            int width,
            int height,
            string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);

            InitBrush();

            //
            // Screen shader.
            //

            using (Shader vertexShader = new Shader(ShaderType.VertexShader, "Shaders\\screen.vert"))
            using (Shader fragmentShader = new Shader(ShaderType.FragmentShader, "Shaders\\screen.frag"))
            {
                this.screenShader = this.tracker.Add(new ShaderProgram(vertexShader, fragmentShader));
            }

            this.screenShader.UseProgram();
            this.screenShader.Uniform("inputTexture", 0);

            //
            // Screen quad VBO.
            //

            this.screenQuadVAO = this.tracker.Add(new VertexArrayObject());
            this.screenQuadVBO = this.tracker.Add(new VertexBufferObject());
            this.screenQuadVAO.BindVertexArray();
            this.screenQuadVBO.BindBuffer(BufferTarget.ArrayBuffer);
            VertexBufferObject.BufferData(BufferTarget.ArrayBuffer, this.screenQuadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(this.screenShader.GetAttributeLocation("aPos"));
            GL.VertexAttribPointer(this.screenShader.GetAttributeLocation("aPos"), 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(this.screenShader.GetAttributeLocation("aTexCoords"));
            GL.VertexAttribPointer(this.screenShader.GetAttributeLocation("aTexCoords"), 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //
            // Scene shader.
            //

            using (Shader vertexShader = new Shader(ShaderType.VertexShader, "Shaders\\shader.vert"))
            using (Shader fragmentShader = new Shader(ShaderType.FragmentShader, "Shaders\\shader.frag"))
            {
                this.shaderProgram = this.tracker.Add(new ShaderProgram(vertexShader, fragmentShader));
            }

            this.shaderProgram.UseProgram();

            //
            // Rendering surface VBO.
            //

            this.surfaceVAO = this.tracker.Add(new VertexArrayObject());
            this.surfaceVBO = this.tracker.Add(new VertexBufferObject());
            this.surfaceVAO.BindVertexArray();
            this.surfaceVBO.BindBuffer(BufferTarget.ArrayBuffer);
            VertexBufferObject.BufferData(BufferTarget.ArrayBuffer, this.surfaceQuadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(this.shaderProgram.GetAttributeLocation("aPos"));
            GL.VertexAttribPointer(this.shaderProgram.GetAttributeLocation("aPos"), 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(this.shaderProgram.GetAttributeLocation("aTexCoords"));
            //GL.VertexAttribPointer(this.shaderProgram.GetAttributeLocation("aTexCoords"), 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //
            // Frame buffers
            //

            // Generate textures for frame buffers.
            this.textureFramebuffer0 = new Texture(this.Width, this.Height);
            this.textureFramebuffer0.BindTexture(0);

            this.framebufferObject0 = this.tracker.Add(new FramebufferObject());
            this.framebufferObject0.BindFramebuffer(FramebufferTarget.DrawFramebuffer);
            GL.FramebufferTexture2D(
                target: FramebufferTarget.DrawFramebuffer,
                attachment: FramebufferAttachment.ColorAttachment0,
                textarget: TextureTarget.Texture2D,
                texture: this.textureFramebuffer0,
                level: 0);
            FramebufferObject.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            // Generate textures for frame buffers.
            this.textureFramebuffer1 = new Texture(this.Width, this.Height);
            this.textureFramebuffer1.BindTexture(0);

            this.framebufferObject1 = this.tracker.Add(new FramebufferObject());
            this.framebufferObject1.BindFramebuffer(FramebufferTarget.DrawFramebuffer);
            GL.FramebufferTexture2D(
                target: FramebufferTarget.DrawFramebuffer,
                attachment: FramebufferAttachment.ColorAttachment0,
                textarget: TextureTarget.Texture2D,
                texture: this.textureFramebuffer1,
                level: 0);
            FramebufferObject.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            Disposable.Dispose(this.textureFramebuffer0);
            Disposable.Dispose(this.textureFramebuffer1);
            Disposable.Dispose(this.tracker);

            base.OnUnload(e);
        }

        private void DoResize()
        {
            GL.Viewport(0, 0, this.Width, this.Height);

            Disposable.Dispose(this.textureFramebuffer0);
            Disposable.Dispose(this.textureFramebuffer1);

            this.textureFramebuffer0 = new Texture(this.Width, this.Height);
            this.framebufferObject0.BindFramebuffer(FramebufferTarget.DrawFramebuffer);
            GL.FramebufferTexture2D(
                target: FramebufferTarget.DrawFramebuffer,
                attachment: FramebufferAttachment.ColorAttachment0,
                textarget: TextureTarget.Texture2D,
                texture: this.textureFramebuffer0,
                level: 0);
            FramebufferObject.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            this.textureFramebuffer1 = new Texture(this.Width, this.Height);
            this.framebufferObject1.BindFramebuffer(FramebufferTarget.DrawFramebuffer);
            GL.FramebufferTexture2D(
                target: FramebufferTarget.DrawFramebuffer,
                attachment: FramebufferAttachment.ColorAttachment0,
                textarget: TextureTarget.Texture2D,
                texture: this.textureFramebuffer1,
                level: 0);
            FramebufferObject.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            InitBrush();
        }

        protected override void OnMove(EventArgs e)
        {
            DoResize();

            base.OnMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            DoResize();

            base.OnResize(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (this.WindowState == WindowState.Fullscreen)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.Exit();
                }
            }

            base.OnKeyUp(e);
        }

        bool swapFramebuffers = false;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            FramebufferObject framebufferObjectA;
            FramebufferObject framebufferObjectB;
            Texture textureA = null;
            Texture textureB;

            this.time += (float)e.Time;
            // float deltaTime = (float)e.Time * this.timeMultiplier;
            float deltaTime = Math.Min(this.maxAllowedDeltaTime, (float)e.Time / this.renderIterationCount);
            this.frameCounter++;

            //float currentFeed = this.feed + (float)Math.Cos(this.time / 1.9) * this.feedMagnitude;
            //float currentKill = this.kill + (float)Math.Cos(this.time / 2.1) * this.killMagnitude;

            float currentFeed = this.feed;
            float currentKill = this.kill;

            Vector2 warpedFeedKill = this.feedKillWarp.Warp(currentFeed, currentKill);
            float warpedFeed = warpedFeedKill.X;
            float warpedKill = warpedFeedKill.Y;

            if (this.isWalking)
            {
                this.walkDurationRemaining -= (float)e.Time;
                this.feed += this.walkFeedIncrementPerSecond * (float)e.Time;
                this.feed = MathHelper.Clamp(this.feed, 0.0f, 1.0f);
                this.kill += this.walkKillIncrmentPerSecond * (float)e.Time;
                this.kill = MathHelper.Clamp(this.kill, 0.0f, 1.0f);

                if (this.walkDurationRemaining <= 0)
                {
                    this.walkDurationRemaining = this.walkSegmentDurationSeconds;
                    this.walkTargetFeed = (float)this.random.NextDouble();
                    this.walkTargetKill = (float)this.random.NextDouble();
                    this.walkFeedIncrementPerSecond = (this.walkTargetFeed - this.feed) / this.walkSegmentDurationSeconds;
                    this.walkKillIncrmentPerSecond = (this.walkTargetKill - this.kill) / this.walkSegmentDurationSeconds;
                }
            }

            if (this.time - this.lastFpsTime > 1)
            {
                Console.WriteLine(
                    $"time: {this.time:00.000}; " +
                    $"FPS: {this.frameCounter:000}; " +
                    $"Rend: {this.renderIterationCount:000}; " +
                    $"FPS * Rend: {this.frameCounter * this.renderIterationCount}; " +
                    $"Feed: {this.feed:0.000}; " +
                    $"Kill: {this.kill:0.000}; " +
                    $"WarpedFeed: {warpedFeed:0.0000}; " +
                    $"WarpedKill: {warpedKill:0.0000}; " +
                    $"Time: {this.timeMultiplier:00.0}; " +
                    $"diffAB: {this.diffusion.X:0.000}, {this.diffusion.Y:0.000}; " +
                    $"eTime: {e.Time:0.00000}; " +
                    $"max(dTime): {this.maxObservedDeltaTime:0.000000}");

                if (this.frameCounter < 60)
                {
                    this.renderIterationCount = Math.Max(1, this.renderIterationCount - this.renderIterationCountIncrement);
                }
                else if (this.frameCounter > 60)
                {
                    this.renderIterationCount = Math.Min(1000, this.renderIterationCount + this.renderIterationCountIncrement);
                }

                this.frameCounter = 0;
                this.lastFpsTime = this.time;
                this.maxObservedDeltaTime = 0f;
            }

            this.maxObservedDeltaTime = Math.Max(deltaTime, this.maxObservedDeltaTime);

            for (int i = 0; i < this.renderIterationCount; i++)
            {
                //
                // Render to framebuffer.
                //

                if (this.swapFramebuffers)
                {
                    framebufferObjectA = this.framebufferObject1;
                    textureA = this.textureFramebuffer1;
                    framebufferObjectB = this.framebufferObject0;
                    textureB = this.textureFramebuffer0;
                }
                else
                {
                    framebufferObjectA = this.framebufferObject0;
                    textureA = this.textureFramebuffer0;
                    framebufferObjectB = this.framebufferObject1;
                    textureB = this.textureFramebuffer1;
                }
                this.swapFramebuffers = !this.swapFramebuffers;

                // Render to texture frame buffer A.
                framebufferObjectA.BindFramebuffer(FramebufferTarget.DrawFramebuffer);
                GL.Enable(EnableCap.DepthTest);
                GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // GL.MemoryBarrier(MemoryBarrierFlags.FramebufferBarrierBit);

                //
                // Draw scene
                //

                this.shaderProgram.UseProgram();

                textureB.BindTexture(0);
                this.shaderProgram.Uniform("brush", this.brush);
                if (this.mouseIsDragging)
                {
                    this.shaderProgram.Uniform("brushVelocity", this.currentMousePosition - this.lastMousePosition);
                }
                this.shaderProgram.Uniform("deltaTime", Math.Min(this.maxAllowedDeltaTime, deltaTime * this.timeMultiplier));
                this.shaderProgram.Uniform("diffusion", this.diffusion);
                this.shaderProgram.Uniform("feed", warpedFeed);
                this.shaderProgram.Uniform("inputTexture", 0);
                this.shaderProgram.Uniform("kill", warpedKill);
                this.shaderProgram.Uniform("laplacian", this.laplacian);
                this.shaderProgram.Uniform("screenResolution", new Vector2(this.Width, this.Height));
                this.shaderProgram.Uniform("time", this.time);
                this.surfaceVAO.BindVertexArray();
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                // GL.MemoryBarrier(MemoryBarrierFlags.FramebufferBarrierBit);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            //
            // Render output buffer to screen.
            // 

            // Bind back to default framebuffer.
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            // Disable depth test as it's not needed.
            GL.Disable(EnableCap.DepthTest);
            // Clear the output buffers.
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            this.screenShader.UseProgram();
            this.screenShader.Uniform("screenResolution", new Vector2(this.Width, this.Height));
            this.screenShader.Uniform("time", time);
            textureA.BindTexture(0);
            this.screenQuadVAO.BindVertexArray();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            this.Context.SwapBuffers();

            base.OnRenderFrame(e);

            this.lastMousePosition = new Vector2(
                this.currentMousePosition.X,
                this.currentMousePosition.Y);

            ResetBrush();
        }

        private void InitBrush()
        {
            this.brush = new Vector4(0.0f, 0.0f, 0.0f, this.brushWipeCanvas);
        }

        private void PaintWithBrush(float x, float y)
        {
            this.brush = new Vector4(
                x / this.Width,
                (this.Height - y) / this.Height,
                0.0f,
                this.brushPaintDot);

            this.currentMousePosition = new Vector2(
                x / this.Width,
                (this.Height - y) / this.Height);
        }

        private void ResetBrush()
        {
            this.brush.W = 0.0f;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (this.mouseIsDragging)
            {
                if (e.Mouse.LeftButton == ButtonState.Pressed)
                {
                    PaintWithBrush(e.Mouse.X, e.Mouse.Y);
                }
                else
                {
                    ResetBrush();
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
            {
                PaintWithBrush(e.Mouse.X, e.Mouse.Y);
                this.mouseIsDragging = true;
            }
            else
            {
                ResetBrush();
            }

            this.lastMousePosition = new Vector2(
                (float)e.Mouse.X / this.Width,
                (float)(this.Height - e.Mouse.Y) / this.Height);

            base.OnMouseUp(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'w':
                    this.isWalking = !this.isWalking;
                    if (this.isWalking)
                    {
                        this.walkDurationRemaining = 0;
                    }
                    break;

                case 'F':
                    this.feed = Math.Min(1.0f, this.feed + this.feedIncrement);
                    break;

                case 'f':
                    this.feed = Math.Max(0.0f, this.feed - this.feedIncrement);
                    break;

                case 'K':
                    this.kill = Math.Min(1.0f, this.kill + this.killIncrement);
                    break;

                case 'k':
                    this.kill = Math.Max(0.0f, this.kill - this.killIncrement);
                    break;

                case 'T':
                    this.timeMultiplier = Math.Min(100.0f, this.timeMultiplier + this.timeMultiplierIncrement);
                    break;

                case 't':
                    this.timeMultiplier = Math.Max(0.0f, this.timeMultiplier - this.timeMultiplierIncrement);
                    break;

                case 'R':
                    this.renderIterationCount = Math.Min(1000, this.renderIterationCount + this.renderIterationCountIncrement);
                    break;

                case 'r':
                    this.renderIterationCount = Math.Max(1, this.renderIterationCount - this.renderIterationCountIncrement);
                    break;

                case 'A':
                    this.diffusion.X = Math.Min(1.0f, this.diffusion.X + this.diffusionIncrement);
                    break;

                case 'a':
                    this.diffusion.X = Math.Max(0.0f, this.diffusion.X - this.diffusionIncrement);
                    break;

                case 'B':
                    this.diffusion.Y = Math.Min(1.0f, this.diffusion.Y + this.diffusionIncrement);
                    break;

                case 'b':
                    this.diffusion.Y = Math.Max(0.0f, this.diffusion.Y - this.diffusionIncrement);
                    break;

                case ' ':
                    DoResize();
                    break;

                case '.':
                    this.brush = new Vector4(0.5f, 0.5f, 0.0f, this.brushPaintDot);
                    break;

                case 'n':
                    this.brush = new Vector4((float)this.random.Next(), (float)this.random.Next(), (float)this.random.Next(), this.brushPaintNoise);
                    break;

                case '1':
                case '2':
                case '3':
                case '4':
                    this.laplacian = 1 + e.KeyChar - '1';
                    break;
            }

            base.OnKeyPress(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouseIsDragging = false;

            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnWindowStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Fullscreen;
            }

            base.OnWindowStateChanged(e);
        }
    }
}
