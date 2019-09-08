//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using mulova.comunity;
using mulova.commons;

namespace UnityEngine.Ex
{
	public static class TransformEx
	{
		
		public static void Set(this Transform t, Transform val)
		{
			t.position = val.position;
			t.rotation = val.rotation;
			Transform p = t.parent;
			if (p == null)
			{
				t.localScale = val.lossyScale;
			} else
			{
				Vector3 vscale = val.lossyScale;
				Vector3 pscale = p.lossyScale;
				if (pscale != Vector3.zero)
				{
					t.localScale = new Vector3(vscale.x / pscale.x, vscale.y / pscale.y, vscale.z / pscale.z);
				}
			}
		}
		
		public static void Set(this Transform t, TransformData d)
		{
			t.position = d.pos;
			t.rotation = d.rot;
			t.localScale = d.scale.Divide(t.parent.lossyScale);
		}
		
		public static void SetLocal(this Transform t, TransformData d)
		{
			t.localPosition = d.pos;
			t.localRotation = d.rot;
			t.localScale = d.scale;
		}
		
		public static void SetLocal(this Transform t, Vector3 localPos, Quaternion localRot, Vector3 localScale)
		{
			t.localPosition = localPos;
			t.localRotation = localRot;
			t.localScale = localScale;
		}
		
		public static void SetLocal(this Transform t, Transform that)
		{
			t.localPosition = that.localPosition;
			t.localRotation = that.localRotation;
			t.localScale = that.localScale;
		}
		
		public static void SetLocalScaleX(this Transform t, float x)
		{
			Vector3 v = t.localScale;
			v.x = x;
			t.localScale = v;
		}
		
		public static void SetLocalScaleY(this Transform t, float y)
		{
			Vector3 v = t.localScale;
			v.y = y;
			t.localScale = v;
		}
		
		public static void SetLocalScaleZ(this Transform t, float z)
		{
			Vector3 v = t.localScale;
			v.z = z;
			t.localScale = v;
		}
		
		public static void AddLocalScaleX(this Transform t, float x)
		{
			Vector3 v = t.localScale;
			v.x += x;
			t.localScale = v;
		}
		
		public static void AddLocalScaleY(this Transform t, float y)
		{
			Vector3 v = t.localScale;
			v.y += y;
			t.localScale = v;
		}
		
		public static void AddLocalScaleZ(this Transform t, float z)
		{
			Vector3 v = t.localScale;
			v.z += z;
			t.localScale = v;
		}
		
		public static void SetLocalPositionX(this Transform t, float x)
		{
			Vector3 v = t.localPosition;
			v.x = x;
			t.localPosition = v;
		}
		
		public static void SetLocalPositionY(this Transform t, float y)
		{
			Vector3 v = t.localPosition;
			v.y = y;
			t.localPosition = v;
		}
		
		public static void SetLocalPositionZ(this Transform t, float z)
		{
			Vector3 v = t.localPosition;
			v.z = z;
			t.localPosition = v;
		}
		
		public static void AddLocalPositionX(this Transform t, float x)
		{
			Vector3 v = t.localPosition;
			v.x += x;
			t.localPosition = v;
		}
		
		public static void AddLocalPositionY(this Transform t, float y)
		{
			Vector3 v = t.localPosition;
			v.y += y;
			t.localPosition = v;
		}
		
		public static void AddLocalPositionZ(this Transform t, float z)
		{
			Vector3 v = t.localPosition;
			v.z += z;
			t.localPosition = v;
		}
		
		public static Transform Search(this Transform trans, string path)
		{
			return trans.Search(path.Split(new char[] {'/'}, System.StringSplitOptions.RemoveEmptyEntries));
		}
		
		/**
		* @param path '/' separated names
		*/
		public static Transform Search(this Transform trans, string[] paths)
		{
			Transform t = trans;
			int i = 0;
			while (i<paths.Length && t != null)
			{
				bool found = false;
				foreach (Transform child in t)
				{
					if (child.name == paths[i])
					{
						t = child;
						found = true;
						break;
					}
				}
				if (!found)
				{
					return null;
				}
				i++;
			}
			return t;
		}
		
		public static Transform BreadthFirstSearch(this Transform trans, string name)
		{
			return BreadthFirstSearch(trans, t => t.name == name);
		}
		
		public static Transform BreadthFirstSearch(this Transform trans, Predicate<Transform> predicate)
		{
			Queue<Transform> q = new Queue<Transform>();
			q.Enqueue(trans);
			while (q.Count > 0)
			{
				Transform child = q.Dequeue();
				if (predicate(child))
				{
					return child;
				}
				for (int i=0; i<child.childCount; i++)
				{
					q.Enqueue(child.GetChild(i));
				}
			}
			return null;
		}
		
		/// <summary>
		/// search the tree by BFS
		/// </summary>
		/// <param name="trans">root transform</param>
		/// <param name="traverse">traverse child depth if one of parent depth returns true</param>
		public static void BreadthFirstTraversal(this Transform trans, Predicate<Transform> traverse)
		{
			Queue<Transform> q = new Queue<Transform>();
			q.Enqueue(trans);
			while (q.Count > 0)
			{
				Transform child = q.Dequeue();
				bool goFurther = traverse(child);
				if (goFurther)
				{
					for (int i=0; i<child.childCount; i++)
					{
						q.Enqueue(child.GetChild(i));
					}
				}
			}
		}
		
		public static Transform DepthFirstSearch(this Transform trans, string name)
		{
			return DepthFirstSearch(trans, t => t.name == name);
		}
		
		public static Transform DepthFirstSearch(this Transform trans, Predicate<Transform> predicate)
		{
			LinkedList<Transform> q = new LinkedList<Transform>();
			q.AddFirst(trans);
			while (q.Count > 0)
			{
				Transform n = q.First.Value;
				q.RemoveFirst();
				if (predicate(n))
				{
					return n;
				}
				for (int i=n.childCount-1; i>=0; --i)
				{
					q.AddFirst(n.GetChild(i));
				}
			}
			return null;
		}
		
		/// <summary>
		/// Traverse the tree by DFS
		/// </summary>
		/// <param name="trans">root transform</param>
		/// <param name="traverse">traverse the child tree if true</param>
		public static void DepthFirstTraversal(this Transform trans, Predicate<Transform> traverse)
		{
			LinkedList<Transform> q = new LinkedList<Transform>();
			q.AddFirst(trans);
			while (q.Count > 0)
			{
				Transform n = q.First.Value;
				q.RemoveFirst();
				bool goFurther = traverse(n);
				if (goFurther)
				{
					for (int i=n.childCount-1; i>=0; --i)
					{
						q.AddFirst(n.GetChild(i));
					}
				}
			}
		}
		
		public static void PreOrderTraversal(this Transform trans, Apply<Transform> traverse)
		{
			PreOrderTraversal(trans, traverse.Apply);
		}
		
		/// <summary>
		/// child를 먼저 방문하고 parent를 나중에 방문한다.
		/// </summary>
		/// <param name="trans">Trans.</param>
		/// <param name="traverse">Traverse.</param>
		public static void PreOrderTraversal(this Transform trans, Action<Transform> traverse)
		{
			for (int i=0; i<trans.childCount; i++)
			{
				PreOrderTraversal(trans.GetChild(i), traverse);
			}
			traverse(trans);
		}
		
		public static GameObject[] ListObjects(this Transform trans)
		{
			Transform[] t = trans.GetComponentsInChildren<Transform>(true);
			List<GameObject> list = new List<GameObject>();
			for (int i=0; i < t.Length; i++)
			{
				GameObject obj = t[i].gameObject;
				if (obj != null)
				{
					list.Add(obj);
				}
			}
			return list.ToArray();
			//      GameObject[] arr = new GameObject[t.Length];
			//      for (int i=0; i < t.Length; i++) {
			//          arr[i] = t[i].gameObject;
			//      }
			//      return arr;
		}
		
		public static Dictionary<string, Transform> CreateTransformMap(this Transform root)
		{
			Transform[] all = root.GetComponentsInChildren<Transform>(true);
			Dictionary<string, Transform> map = new Dictionary<string, Transform>();
			foreach (Transform t in all)
			{
				map[t.name] = t;
			}
			return map;
		}
		
		public static void SetLayerRecursively(this Transform t, int layer)
		{
			if (t == null)
				return;
			t.gameObject.layer = layer;
			foreach (Transform tChild in t)
			{
				SetLayerRecursively(tChild, layer);
			}
		}
		
		public static void SetLocalPosition(this Transform t, Vector3 pos, float tolerance)
		{
			if (Platform.isEditor)
			{
				if (!t.localPosition.Equals(pos, tolerance))
				{
					t.localPosition = pos;
				}
			} else
			{
				t.localPosition = pos;
			}
		}
		
		public static void SetPosition(this Transform t, Vector3 pos, float tolerance)
		{
			if (Platform.isEditor)
			{
				if (!t.position.Equals(pos, tolerance))
				{
					t.position = pos;
				}
			} else
			{
				t.position = pos;
			}
		}
		
		public static void SetLocalScale(this Transform t, Vector3 pos, float tolerance)
		{
			if (Platform.isEditor)
			{
				if (!t.localScale.Equals(pos, tolerance))
				{
					t.localScale = pos;
				}
			} else
			{
				t.localScale = pos;
			}
		}
		
		/// <summary>
		/// Convert v from current space to target space
		/// </summary>
		/// <returns>The space.</returns>
		/// <param name="currentSpace">Current space.</param>
		/// <param name="v">V.</param>
		/// <param name="targetSpace">Target space.</param>
		public static Vector3 TransformSpace(this Transform currentSpace, Vector3 v, Transform targetSpace)
		{
			if (targetSpace != currentSpace)
			{
				Matrix4x4 toLocal = targetSpace.worldToLocalMatrix;
				// Transform the coordinate from relative-to-widget to world space
				v = currentSpace.TransformPoint(v);
				// Now transform from world space to relative-to-parent space
				v = toLocal.MultiplyPoint3x4(v);
			}
			return v;
		}
		
		public static string GetScenePath(this Transform trans)
		{
			return trans.GetScenePath(null);
		}
		
		public static string GetScenePath(this Transform trans, Transform root)
		{
			Stack<Transform> stack = new Stack<Transform>();
			Transform t = trans;
			while (t != null && t != root)
			{
				stack.Push(t);
				t = t.parent;
			}
			StringBuilder str = new StringBuilder();
			while (stack.Count > 0)
			{
				str.Append('/').Append(stack.Pop().name);
			}
			return str.ToString();
		}
		
		public static Bounds TransformBounds(this Transform target, Transform srcTrans, Bounds srcBound)
		{
			if (srcTrans == target)
			{
				return srcBound;
			}
			Bounds dst = new Bounds();
			dst.center = srcTrans.TransformSpace(srcBound.center, target);
			dst.size = Vector3.Scale(srcBound.size, VectorUtil.Divide(target.lossyScale, srcTrans.lossyScale));
			return dst;
		}
		
		public static Rect TransformRect(this Transform target, Transform srcTrans, Rect srcRect)
		{
			if (srcTrans == target)
			{
				return srcRect;
			}
			Bounds src = new Bounds(new Vector3(srcRect.center.x, srcRect.center.y, 0), new Vector3(srcRect.width, srcRect.height, 0));
			Bounds dst = target.TransformBounds(srcTrans, src);
			Rect r = new Rect();
			r.center = dst.center;
			r.size = dst.size;
			return r;
		}
		
		public static void SetParentEx(this Transform t, Transform parent, bool worldPositionStays)
		{
			t.SetParent(parent, worldPositionStays);
			t.gameObject.SetLayer(parent.gameObject.layer);
		}
	}
}
