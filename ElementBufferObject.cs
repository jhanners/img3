using OpenTK.Graphics.OpenGL;
using System;
using System.CodeDom;

namespace LearnOpenGL
{
    public class ElementBufferObject : Disposable
    {
        public int ElementBuffer { get; private set; }

        public ElementBufferObject()
        {
            this.ElementBuffer = GL.GenBuffer();
            GLChk.GetError();
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Do nothing.
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteBuffer(this.ElementBuffer);
        }

        #endregion

        #region Operators

        public static implicit operator int(ElementBufferObject value) => value.ElementBuffer;
        
        #endregion

        #region GL pass-through

        public void BindBuffer(BufferTarget target)
        {
            GL.BindBuffer(target, this.ElementBuffer);
            GLChk.GetError();
        }

        public void BufferData(BufferTarget target, int[] data, BufferUsageHint hint)
        {
            ChkArg.IsNotNull(data, nameof(data));
            ChkArg.IsGreaterThan(data.Length, 0, nameof(data));
            GL.BufferData(target, data.Length * sizeof(int), data, hint);
            GLChk.GetError();
        }

        #endregion
    }
}
