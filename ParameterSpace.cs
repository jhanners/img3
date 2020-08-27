using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace img3
{
    class ParameterSpace
    {
        private class Slice
        {
            public float U;
            public float V0;
            public float V1;

            public Slice(
                float u,
                float v0,
                float v1)
            {
                this.U = u;
                this.V0 = v0;
                this.V1 = v1;
            }
        }

        private List<Slice> slices = new List<Slice>();

        public ParameterSpace()
        {
            // AddSlice(0.006f, 0.031f, 0.040f);
            AddSlice(0.0160f, 0.0420f, 0.0530f);
            AddSlice(0.0220f, 0.0475f, 0.0576f);
            AddSlice(0.0420f, 0.0586f, 0.0655f);
            AddSlice(0.0620f, 0.0612f, 0.0680f);
            AddSlice(0.0820f, 0.0590f, 0.0598f);
            AddSlice(0.1000f, 0.0550f, 0.0590f);
        }

        private void AddSlice(float u, float v0, float v1)
        {
            this.slices.Add(new Slice(u, v0, v1));
        }

        public Vector2 Warp(float u, float v)
        {
            if (u < 0f || u > 1.0f)
            {
                u = MathHelpers.Fraction(u);
            }
            if (v < 0f || v > 1.0f)
            {
                v = MathHelpers.Fraction(v);
            }

            float minU = this.slices[0].U;
            float maxU = this.slices[this.slices.Count - 1].U;
            float deltaU = maxU - minU;

            u *= deltaU;
            u += minU;
            int i = 0;
            Slice slice0 = this.slices[0];
            Slice slice1 = this.slices[1];
            while (true)
            {
                if (u >= slice0.U && u <= slice1.U)
                {
                    break;
                }
                i++;
                if (i >= this.slices.Count)
                {
                    break;
                }
                slice0 = slice1;
                slice1 = this.slices[i + 1];
            }
            deltaU = slice1.U - slice0.U;
            u -= slice0.U;
            u /= deltaU;

            Vector2 deltaV0 = new Vector2(slice1.U - slice0.U, slice1.V0 - slice0.V0);
            Vector2 deltaV1 = new Vector2(slice1.U - slice0.U, slice1.V1- slice0.V1);

            deltaV0 *= u;
            deltaV1 *= u;
            Vector2 deltaV01 = new Vector2(0.0f, slice0.V1 - slice0.V0) + deltaV1 - deltaV0;
            deltaV01 *= v;

            Vector2 result = new Vector2(slice0.U, slice0.V0) + deltaV0 + deltaV01;

            return result;
        }
    }
}
