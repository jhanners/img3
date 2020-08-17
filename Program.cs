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
            using (Window window = new Window(1280, 1024, "img3"))
            {
                window.Run();
            }
        }
    }
}
