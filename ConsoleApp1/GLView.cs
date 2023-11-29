#if !NET40&&!NETCOREAPP3_0
using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Skia;
using CPF.Styling;
using CPF.Svg;
using CPF.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using Silk.NET.OpenGLES;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://$safeprojectname$/Stylesheet1.css")]//用于设计的时候加载样式
    public class GLView : UIElement
    {
        GL _gl;
        public uint Id { get; set; }

        public uint ColorBuffer { get; set; }

        public uint DepthRenderBuffer { get; set; }
        Size oldSize;
        SKImage image;

        uint vao;
        uint shaderProgram;
        //IGlContext context;
        protected unsafe override void OnRender(DrawingContext dc)
        {
            var size1 = ActualSize;
            if (size1.Width <= 0 || size1.Height <= 0 || DesignMode)
            {
                return;
            }

            var size = new PixelSize((int)Math.Round(size1.Width * Root.RenderScaling), (int)Math.Round(size1.Height * Root.RenderScaling));
            var skia = dc as SkiaDrawingContext;
            var gl = skia.GlContext;
            //context = gl;
            OpenglEx.Load(gl);
            if (_gl == null)
            {
                _gl = GL.GetApi(gl.GetProcAddress);
                Id = _gl.GenFramebuffer();
                ColorBuffer = _gl.GenTexture();
                DepthRenderBuffer = _gl.GenRenderbuffer();

                _gl.BindTexture(GLEnum.Texture2D, ColorBuffer);

                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);

                _gl.BindTexture(GLEnum.Texture2D, 0);


                _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                float[] vertices = {
                -0.5f, -0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f
                };

                _gl.GenBuffers(1, out uint vbo);
                _gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
                _gl.BufferData<float>(GLEnum.ArrayBuffer, (nuint)vertices.Length * sizeof(float), vertices, GLEnum.StaticDraw);

                _gl.GenVertexArrays(1, out vao);
                _gl.BindVertexArray(vao);
                _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), null);
                _gl.EnableVertexAttribArray(0);

                string vertexShaderSource = @"#version 330 core
                layout (location = 0) in vec3 aPos;
                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                }
            ";

                string fragmentShaderSource = @"#version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
                }
            ";

                uint vertexShader = _gl.CreateShader(GLEnum.VertexShader);
                _gl.ShaderSource(vertexShader, vertexShaderSource);
                _gl.CompileShader(vertexShader);

                uint fragmentShader = _gl.CreateShader(GLEnum.FragmentShader);
                _gl.ShaderSource(fragmentShader, fragmentShaderSource);
                _gl.CompileShader(fragmentShader);

                shaderProgram = _gl.CreateProgram();
                _gl.AttachShader(shaderProgram, vertexShader);
                _gl.AttachShader(shaderProgram, fragmentShader);
                _gl.LinkProgram(shaderProgram);

                _gl.DeleteShader(vertexShader);
                _gl.DeleteShader(fragmentShader);
            }
            if (size1 != oldSize)
            {
                oldSize = size1;

                _gl.BindTexture(GLEnum.Texture2D, ColorBuffer);
                _gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)size.Width, (uint)size.Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, null);
                _gl.BindTexture(GLEnum.Texture2D, 0);

                _gl.BindRenderbuffer(GLEnum.Renderbuffer, DepthRenderBuffer);
                _gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth32fStencil8, (uint)size.Width, (uint)size.Height);

                _gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

                _gl.BindFramebuffer(GLEnum.Framebuffer, Id);

                _gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, ColorBuffer, 0);

                _gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, DepthRenderBuffer);
                _gl.BindFramebuffer(GLEnum.Framebuffer, 0);


                GRBackendTexture backendTexture = new GRBackendTexture((int)(size.Width / Root.RenderScaling), (int)(size.Height / Root.RenderScaling), false, new GRGlTextureInfo(0x0DE1, (uint)ColorBuffer, SKColorType.Rgba8888.ToGlSizedFormat()));

                image = SKImage.FromTexture((GRContext)skia.GlContext.GRContext, backendTexture, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
            }

            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
            var vp = new float[4];
            gl.GetFloatv(GlConsts.GL_VIEWPORT, vp);
            _gl.Viewport(0, 0, (uint)size.Width, (uint)size.Height);
            OnGlRender(gl, size);
            _gl.Viewport((int)vp[0], (int)vp[1], (uint)vp[2], (uint)vp[3]);
            skia.SKCanvas.DrawImage(image, 0, 0);
            base.OnRender(dc);
        }

        Random random = new Random();
        protected void OnGlRender(IGlContext gl, PixelSize viewPort)
        {
            //_gl.ClearColor(0.5f, 0, 0, 0.5f);
            //_gl.Clear(ClearBufferMask.ColorBufferBit);
            _gl.UseProgram(shaderProgram);
            _gl.BindVertexArray(vao);
            _gl.DrawArrays(GLEnum.Triangles, 0, 3);
        }

        protected override void Dispose(bool disposing)
        {
            if (_gl != null)
            {
                OpenglEx.DeleteFramebuffers(null, 1, new int[] { (int)Id });
                OpenglEx.DeleteTextures(null, 1, new int[] { (int)ColorBuffer });
                OpenglEx.DeleteRenderbuffers(null, 1, new int[] { (int)DepthRenderBuffer });
                //_gl.DeleteFramebuffer(Id);
                //_gl.DeleteTexture(ColorBuffer);
                //_gl.DeleteRenderbuffer(DepthRenderBuffer);
            }
            base.Dispose(disposing);
        }
    }
}
#endif