using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UIPolygon
{
    public class UIPolygon : MaskableGraphic
    {
        [System.Serializable]
        public class Polygon
        {
            public Vector2 Offset;
            public Vector2[] Points;
            public Color Color;
        }

        public Polygon[] PathList = new[]
        {
            new Polygon()
            {
                Points = new []
                {
                    new Vector2(-100, -100),
                    new Vector2(100, -100),
                    new Vector2(0, 100),
                },
                Color = Color.yellow,
            },
        };

        public override Texture mainTexture
        {
            get
            {
                return s_WhiteTexture;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (PathList == null)
            {
                return;
            }

            int offset = 0;
            foreach (var path in PathList)
            {
                var points = path.Points.Select(p => p + path.Offset).ToArray();
                foreach (var p in points)
                {
                    vh.AddVert(p, path.Color, Vector2.zero);
                }
                var triangles = new Triangulator(points).Triangulate();
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    vh.AddTriangle(offset + triangles[i], offset + triangles[i + 1], offset + triangles[i + 2]);
                }
                offset += path.Points.Length;
            }
        }
    }
}