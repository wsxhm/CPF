#if !NET40
using CPF;
using CPF.Drawing;
using CPF.OpenGL;
using CPF.Skia;
using SkiaSharp;
using System;

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://$safeprojectname$/Stylesheet1.css")]//用于设计的时候加载样式
    public class GLView : CPF.Skia.GLView
    {
#if !NETCOREAPP3_0
        Silk.NET.OpenGLES.GL _gl;

        uint vao;
        uint shaderProgram;

        protected unsafe override void OnGLLoaded(IGlContext gl)
        {
            _gl = Silk.NET.OpenGLES.GL.GetApi(gl.GetProcAddress);
            _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            float[] vertices = {
                -0.5f, -0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f
                };

            _gl.GenBuffers(1, out uint vbo);
            _gl.GenVertexArrays(1, out vao);

            _gl.BindBuffer(Silk.NET.OpenGLES.GLEnum.ArrayBuffer, vbo);
            _gl.BindVertexArray(vao);

            _gl.BufferData<float>(Silk.NET.OpenGLES.GLEnum.ArrayBuffer,vertices, Silk.NET.OpenGLES.GLEnum.StaticDraw);

            _gl.VertexAttribPointer(0, 3, Silk.NET.OpenGLES.GLEnum.Float, false, 3 * sizeof(float), null);
            _gl.EnableVertexAttribArray(0);

            _gl.BindVertexArray(0);
            _gl.BindBuffer(Silk.NET.OpenGLES.GLEnum.ArrayBuffer, 0);

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

            uint vertexShader = _gl.CreateShader(Silk.NET.OpenGLES.GLEnum.VertexShader);
            _gl.ShaderSource(vertexShader, vertexShaderSource);
            _gl.CompileShader(vertexShader);

            uint fragmentShader = _gl.CreateShader(Silk.NET.OpenGLES.GLEnum.FragmentShader);
            _gl.ShaderSource(fragmentShader, fragmentShaderSource);
            _gl.CompileShader(fragmentShader);

            shaderProgram = _gl.CreateProgram();
            _gl.AttachShader(shaderProgram, vertexShader);
            _gl.AttachShader(shaderProgram, fragmentShader);
            _gl.LinkProgram(shaderProgram);

            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);
            base.OnGLLoaded(gl);
        }


        protected override void OnGLRender(IGlContext gl)
        {
            _gl.ClearColor(0.5f, 1, 0, 0.5f);
            _gl.Clear(Silk.NET.OpenGLES.ClearBufferMask.ColorBufferBit | Silk.NET.OpenGLES.ClearBufferMask.DepthBufferBit | Silk.NET.OpenGLES.ClearBufferMask.StencilBufferBit);

            _gl.UseProgram(shaderProgram);

            _gl.BindVertexArray(vao);

            _gl.DrawArrays(Silk.NET.OpenGLES.GLEnum.Triangles, 0, 3);

            _gl.BindVertexArray(0);
            _gl.UseProgram(0);
            base.OnGLRender(gl);
        }

#endif

    }
}
#endif