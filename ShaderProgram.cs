using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace LearnOpenGL
{
    public class ShaderProgram : Disposable
    {
        public int ProgramHandle { get; private set; }

        private Dictionary<string, int> attributeLocations = new Dictionary<string, int>();
        private Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

        public ShaderProgram(
            params Shader[] shaders)
        {
            this.ProgramHandle = GL.CreateProgram();

            foreach (Shader shader in shaders)
            {
                GL.AttachShader(this.ProgramHandle, shader);
                GLChk.GetError();
            }

            GL.LinkProgram(this.ProgramHandle);
            GLChk.GetError();

            GL.GetProgram(this.ProgramHandle, GetProgramParameterName.LinkStatus, out int linkStatus);
            GLChk.GetError();

            if (linkStatus == 0)
            {
                string log = GL.GetProgramInfoLog(this.ProgramHandle);
                if (log == System.String.Empty)
                {
                    log = $"Unknown failure: linkStatus = {linkStatus}";
                }
                Console.WriteLine(log);
                throw new Exception(log);
            }

            foreach (Shader shader in shaders)
            {
                GL.DetachShader(this.ProgramHandle, shader);
                GLChk.GetError();
            }

            Dump();
        }

        public void Dump()
        {
            GL.GetProgramInterface(this.ProgramHandle, ProgramInterface.ProgramInput, ProgramInterfaceParameter.ActiveResources, out int inputCount);
            for (int i = 0; i < inputCount; i++)
            {
                GL.GetProgramResourceName(this.ProgramHandle, ProgramInterface.ProgramInput, i, 1024, out int length, out string name);
                Console.WriteLine($"input[{i}] is {name}");
            }

            GL.GetProgramInterface(this.ProgramHandle, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out int uniformCount);
            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetProgramResourceName(this.ProgramHandle, ProgramInterface.Uniform, i, 1024, out int length, out string name);
                Console.WriteLine($"uniform[{i}] is {name}");
            }

            GL.GetProgramInterface(this.ProgramHandle, ProgramInterface.ProgramOutput, ProgramInterfaceParameter.ActiveResources, out int outputCount);
            for (int i = 0; i < outputCount; i++)
            {
                GL.GetProgramResourceName(this.ProgramHandle, ProgramInterface.ProgramOutput, i, 1024, out int length, out string name);
                Console.WriteLine($"output[{i}] is {name}");
            }
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Nothing to do
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteProgram(this.ProgramHandle);
        }

        #endregion

        #region Operators

        public static implicit operator int(ShaderProgram value) => value.ProgramHandle;

        #endregion

        #region GL pass-through

        public void UseProgram()
        {
            GL.UseProgram(this.ProgramHandle);
            GLChk.GetError();
        }

        public int GetAttributeLocation(string name)
        {
            if (!this.attributeLocations.TryGetValue(name, out int result))
            {
                result = GL.GetAttribLocation(this.ProgramHandle, name);
                if (result < 0)
                {
                    ErrorCode errorCode = GL.GetError();
                    throw new Exception($"Attribute location \"{name}\" not found.  errorCode = {errorCode}");
                }
                this.attributeLocations[name] = result;
            }
            return result;
        }

        public int GetUniformLocation(string name)
        {
            if (!this.uniformLocations.TryGetValue(name, out int result))
            {
                result = GL.GetUniformLocation(this.ProgramHandle, name);
                if (result < 0)
                {
                    ErrorCode errorCode = GL.GetError();
                    Console.WriteLine($"ERROR: Uniform location \"{name}\" not found.  errorCode = {errorCode}");
                    // throw new Exception($"Uniform location \"{name}\" not found.  errorCode = {errorCode}");
                }
                this.uniformLocations[name] = result;
            }
            return result;
        }

        public void Uniform(string name, int value)
        {
            GL.Uniform1(
                location: this.GetUniformLocation(name),
                v0: value);
            GLChk.GetError();
        }

        public void Uniform(string name, float value)
        {
            GL.Uniform1(
                location: this.GetUniformLocation(name),
                v0: value);
            GLChk.GetError();
        }

        public void Uniform(string name, Vector2 value)
        {
            GL.Uniform2(
                location: this.GetUniformLocation(name),
                vector: value);
            GLChk.GetError();
        }

        public void Uniform(string name, Vector4 value)
        {
            GL.Uniform4(
                location: this.GetUniformLocation(name),
                v0: value[0],
                v1: value[1],
                v2: value[2],
                v3: value[3]);
            GLChk.GetError();
        }

        public void Uniform(string name, ref Matrix4 data)
        {
            GL.UniformMatrix4(
                location: this.GetUniformLocation(name),
                transpose: true,
                matrix: ref data);
            GLChk.GetError();
        }

        #endregion
    }
}
