using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Text;

namespace LearnOpenGL
{
    public class Shader : Disposable
    {
        public int ShaderHandle { get; private set; }

        public Shader(ShaderType type, string path)
        {
            string source;

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                source = reader.ReadToEnd();
            }

            this.ShaderHandle = GL.CreateShader(type);
            GLChk.GetError();

            GL.ShaderSource(this.ShaderHandle, source);
            GLChk.GetError();

            GL.CompileShader(this.ShaderHandle);
            GLChk.GetError();

            GL.GetShader(this.ShaderHandle, ShaderParameter.CompileStatus, out int compileStatus);
            GLChk.GetError();

            if (compileStatus == 0)
            {
                string log = GL.GetShaderInfoLog(this.ShaderHandle);
                if (log == System.String.Empty)
                {
                    log = $"Unknown failure: compileStatus = {compileStatus}";
                }
                Console.WriteLine(log);
                throw new Exception(log);
            }
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Nothing to do.
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteShader(this.ShaderHandle);
        }

        #endregion

        #region Operators

        public static implicit operator int(Shader value) => value.ShaderHandle;

        #endregion
    }
}
