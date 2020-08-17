using OpenTK.Graphics.OpenGL;
using System;
using System.CodeDom;

namespace LearnOpenGL
{
    public class VertexArrayObject : Disposable
    {
        public int VertexArray { get; private set; }

        public VertexArrayObject()
        {
            this.VertexArray = GL.GenVertexArray();
            GLChk.GetError();
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Do nothing.
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteVertexArray(this.VertexArray);
        }

        #endregion

        #region Operators

        public static implicit operator int(VertexArrayObject value) => value.VertexArray;

        #endregion

        #region GL pass-through

        public void BindVertexArray()
        {
            GL.BindVertexArray(this.VertexArray);
            GLChk.GetError();
        }

        #endregion
    }
}
