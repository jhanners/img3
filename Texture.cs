using Microsoft.SqlServer.Server;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LearnOpenGL
{
    public class Texture : Disposable
    {
        public int TextureHandle { get; private set; }

        public Texture(string fileName)
        {
            // Generate handle.
            this.TextureHandle = GL.GenTexture();
            GLChk.GetError();

            // Bind it.
            this.BindTexture(0);
            GLChk.GetError();

            // Load the texture from disk.
            using (Bitmap bitmap = new Bitmap(fileName))
            {
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                BitmapData data = bitmap.LockBits(
                    rect: new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    flags: ImageLockMode.ReadOnly,
                    format: System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(
                    target: TextureTarget.Texture2D,
                    level: 0,
                    internalformat: PixelInternalFormat.Rgba,
                    width: bitmap.Width,
                    height: bitmap.Height,
                    border: 0,
                    format: OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    type: PixelType.UnsignedByte,
                    pixels: data.Scan0);
                GLChk.GetError();
            }

            // Generate mipmaps.

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GLChk.GetError();

            // Set min and mag filter.

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMinFilter,
                param: (int)TextureMinFilter.LinearMipmapLinear);
            GLChk.GetError();

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMagFilter,
                param: (int)TextureMinFilter.Linear);
            GLChk.GetError();

            // Set wrapping mode.

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureWrapS,
                param: (int)TextureWrapMode.Repeat);
            GLChk.GetError();

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureWrapT,
                param: (int)TextureWrapMode.Repeat);
            GLChk.GetError();
        }

        public Texture(int width, int height)
        {
            // Generate handle.
            this.TextureHandle = GL.GenTexture();
            GLChk.GetError();

            // Bind it.
            this.BindTexture(0);
            GLChk.GetError();

            GL.TexImage2D(
                target: TextureTarget.Texture2D,
                level: 0,
                internalformat: PixelInternalFormat.Rgba32f,
                width: width,
                height: height,
                border: 0,
                format: OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                type: PixelType.Byte,
                pixels: IntPtr.Zero);
            GLChk.GetError();

            // Set min and mag filter.

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMinFilter,
                param: (int)TextureMinFilter.Linear);
            GLChk.GetError();

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMagFilter,
                param: (int)TextureMinFilter.Linear);
            GLChk.GetError();

            // Set wrapping mode.

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureWrapS,
                param: (int)TextureWrapMode.Repeat);
            GLChk.GetError();

            GL.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureWrapT,
                param: (int)TextureWrapMode.Repeat);
            GLChk.GetError();
        }

        #region Disposable

        protected override void CleanupDisposableObjects()
        {
            // Do nothing.
        }

        protected override void CleanupUnmanagedResources()
        {
            GL.DeleteTexture(this.TextureHandle);
            GLChk.GetError();
        }

        #endregion

        #region Operators

        public static implicit operator int(Texture value) => value.TextureHandle;

        #endregion

        #region GL pass-through

        public void BindTexture(
            int unit)
        {
            this.BindTexture((TextureUnit)(unit + (int)TextureUnit.Texture0));
        }

        public void BindTexture(
            TextureUnit unit)
        {
            GL.ActiveTexture(
                texture: unit);
            GLChk.GetError();

            GL.BindTexture(
                target: TextureTarget.Texture2D,
                texture: this.TextureHandle);
            GLChk.GetError();
        }

        #endregion
    }
}
