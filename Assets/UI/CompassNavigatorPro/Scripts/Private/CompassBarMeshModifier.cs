using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace CompassNavigatorPro {

	public class CompassBarMeshModifier : BaseMeshEffect {

		public override void ModifyMesh (VertexHelper vh) {
			if (!IsActive () || vh == null || vh.currentVertCount < 1)
				return;
			const float maxWidth = 50;
			Mesh m = new Mesh ();
			vh.FillMesh (m);

			List<Vector3> positions = new List<Vector3> ();
			List<int> tris = new List<int> ();
			List<Vector2> uv = new List<Vector2> ();

			m.GetVertices (positions);
			m.GetTriangles (tris, 0);
			m.GetUVs (0, uv);

			List<Vector3> newPositions = new List<Vector3> ();
			List<int> newTris = new List<int> ();
			List<Vector2> newUVs = new List<Vector2> ();

			int triCount = tris.Count;
			int vertexCount = 0;
			for (int t = 0; t < triCount; t += 6) {
				int idx0 = tris [t];
				int idx1 = tris [t + 1];
				int idx2 = tris [t + 2];
				int idx3 = tris [t + 4];

				Vector3 p0 = positions [idx0];
				Vector3 p1 = positions [idx1];
				Vector3 p2 = positions [idx2];
				Vector3 p3 = positions [idx3];
				if (p0.x == p3.x || p0.y == p1.y) {
					continue;
				}
				Vector2 uv0 = uv [idx0];
				Vector2 uv1 = uv [idx1];
				Vector2 uv2 = uv [idx2];
				Vector2 uv3 = uv [idx3];
				float width = p3.x - p0.x;
				float uvWidth = uv3.x - uv0.x;
				int divisions = (int)(width / maxWidth);
				float segmentWidth = width / (divisions + 1);
				float uvSegmentWidth = uvWidth / (divisions + 1);
				for (int k = 0; k < divisions; k++) {
					float midX = p0.x + segmentWidth;
					float midUV = uv0.x + uvSegmentWidth;

					Vector3 midPositionTop = new Vector3 (midX, p1.y, 0);
					Vector2 midUVTop = new Vector2 (midUV, uv1.y);
					Vector3 midPositionBottom = new Vector3 (midX, p0.y, 0);
					Vector2 midUVBottom = new Vector2 (midUV, uv0.y);

					newPositions.Add (p0);
					newPositions.Add (p1);
					newPositions.Add (midPositionTop);
					newPositions.Add (midPositionBottom);
					newUVs.Add (uv0);
					newUVs.Add (uv1);
					newUVs.Add (midUVTop);
					newUVs.Add (midUVBottom);

					// 1st triangle
					newTris.Add (vertexCount);
					newTris.Add (vertexCount + 1);
					newTris.Add (vertexCount + 2);
					// 2nd triangle
					newTris.Add (vertexCount);
					newTris.Add (vertexCount + 2);
					newTris.Add (vertexCount + 3);

					vertexCount += 4;
					p0 = midPositionBottom;
					p1 = midPositionTop;
					uv0 = midUVBottom;
					uv1 = midUVTop;
				}
				// Add last quad
				newPositions.Add (p0);
				newPositions.Add (p1);
				newPositions.Add (p2);
				newPositions.Add (p3);
				newUVs.Add (uv0);
				newUVs.Add (uv1);
				newUVs.Add (uv2);
				newUVs.Add (uv3);

				// 1st triangle
				newTris.Add (vertexCount);
				newTris.Add (vertexCount + 1);
				newTris.Add (vertexCount + 2);
				// 2nd triangle
				newTris.Add (vertexCount);
				newTris.Add (vertexCount + 2);
				newTris.Add (vertexCount + 3);
				vertexCount += 4;
			}


			int newPositionsCount = newPositions.Count;
			List<UIVertex> newVertices = new List<UIVertex> (newPositionsCount);
			UIVertex vx = new UIVertex ();
			vh.PopulateUIVertex (ref vx, 0);
			for (int k = 0; k < newPositionsCount; k++) {
				vx.position = newPositions [k];
				vx.uv0 = newUVs [k];
				newVertices.Add (vx);
			}
			vh.Clear ();
			vh.AddUIVertexStream (newVertices, newTris);
		}

	}

}