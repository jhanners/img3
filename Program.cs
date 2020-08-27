using img3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenGL
{
    class Program
    {
        static void Main(string[] args)
        {
            MathHelpers.Test();
            ParameterSpace ps = new ParameterSpace();
            ps.Warp(0.0f, 1.0f);
            ps.Warp(0.5f, 0.5f);
            using (Window window = new Window(1920, 1080, "img3"))
            {
                window.Run();
            }
        }
    }
}
