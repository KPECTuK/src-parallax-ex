using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[InitializeOnLoad]
	internal static class HierarchyIcons
	{
		// how many objects we want to allow to get updated on select/deselect
		private const int MAX_SELECTION_UPDATE_COUNT = 3;

		// add your components and the associated icons here: https://github.com/halak/unity-editor-icons
		private static readonly Dictionary<Type, GUIContent> _typeIcons = new()
		{
			{ typeof(CameraManager), EditorGUIUtility.IconContent("d_SceneViewTools") },
			{ typeof(Camera), EditorGUIUtility.IconContent("d_SceneViewCamera") },
			{ typeof(OffAxisProjection), EditorGUIUtility.IconContent( "d_SceneViewTools" ) },
		};

		// cached game object information
		private static readonly Dictionary<int, GUIContent> _labeledObjects = new();
		private static readonly HashSet<int> _unlabeledObjects = new();
		private static GameObject[] _previousSelection;

		static HierarchyIcons()
		{
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

			// callbacks for when we want to update the object GUI state:
			ObjectFactory.componentWasAdded += c => UpdateObject(c.gameObject.GetInstanceID());
			// there's no componentWasRemoved callback, but we can use selection as a proxy:
			Selection.selectionChanged += OnSelectionChanged;
		}

		private static void OnHierarchyGUI(int id, Rect rect)
		{
			if(_unlabeledObjects.Contains(id))
			{
				// don't draw things with no component of interest
				return;
			}

			if(ShouldDrawObject(id, out var icon))
			{
				// right-align the icon
				rect.xMin = rect.xMax - 40;
				GUI.Label(rect, icon);
			}
		}

		private static bool ShouldDrawObject(int id, out GUIContent icon)
		{
			// object is unsorted, add it and get icon, if applicable
			return _labeledObjects.TryGetValue(id, out icon) || SortObject(id, out icon);
		}

		private static bool SortObject(int id, out GUIContent icon)
		{
			var go = EditorUtility.InstanceIDToObject(id) as GameObject;
			if(go != null)
			{
				foreach(var (type, typeIcon) in _typeIcons)
				{
					if(go.GetComponent(type))
					{
						_labeledObjects.Add(id, icon = typeIcon);
						return true;
					}
				}
			}

			_unlabeledObjects.Add(id);

			icon = default;
			return false;
		}

		private static void UpdateObject(int id)
		{
			_unlabeledObjects.Remove(id);
			_labeledObjects.Remove(id);

			SortObject(id, out _);
		}

		private static void OnSelectionChanged()
		{
			// update on deselect
			TryUpdateObjects(_previousSelection);
			// update on select
			TryUpdateObjects(_previousSelection = Selection.gameObjects);
		}

		private static void TryUpdateObjects(GameObject[] objects)
		{
			if(objects is { Length: > 0 and <= MAX_SELECTION_UPDATE_COUNT })
			{
				// max of three to prevent performance hitches when selecting many objects
				foreach(var go in objects)
				{
					UpdateObject(go.GetInstanceID());
				}
			}
		}
	}
}
