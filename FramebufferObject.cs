using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Text;

namespace LearnOpenGL
{
    public class FramebufferObject : Disposable
    {
        public int Framebuffer { get; private set; }

        public FramebufferObject()
        {
            this.Framebuffer = GL.GenFramebuffer();
            GLChk.GetError();
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Nothing to do.
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteFramebuffer(this.Framebuffer);
        }

        #endregion

        #region Operators

        public static implicit operator int(FramebufferObject value) => value.Framebuffer;

        #endregion

        #region GL pass-through

        public void BindFramebuffer(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, this.Framebuffer);
            GLChk.GetError();
        }

        public static void CheckFramebufferStatus(FramebufferTarget target)
        {
            FramebufferErrorCode status = GL.CheckFramebufferStatus(target);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Framebuffer {target} status is {status}.");
            }
        }

        #endregion
    }
}
