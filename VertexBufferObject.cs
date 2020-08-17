using OpenTK.Graphics.OpenGL;
using System;
using System.CodeDom;

namespace LearnOpenGL
{
    public class VertexBufferObject : Disposable
    {
        public int VertexBuffer { get; private set; }

        public VertexBufferObject()
        {
            this.VertexBuffer = GL.GenBuffer();
            GLChk.GetError();
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Do nothing.
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteBuffer(this.VertexBuffer);
        }

        #endregion

        #region Operators

        public static implicit operator int(VertexBufferObject value) => value.VertexBuffer;
        
        #endregion

        #region GL pass-through

        public void BindBuffer(BufferTarget target)
        {
            GL.BindBuffer(target, this.VertexBuffer);
            GLChk.GetError();
        }

        public static void BufferData(BufferTarget target, float[] data, BufferUsageHint hint)
        {
            ChkArg.IsNotNull(data, nameof(data));
            ChkArg.IsGreaterThan(data.Length, 0, nameof(data));
            GL.BufferData(target, data.Length * sizeof(float), data, hint);
            GLChk.GetError();
        }

        #endregion
    }
}
