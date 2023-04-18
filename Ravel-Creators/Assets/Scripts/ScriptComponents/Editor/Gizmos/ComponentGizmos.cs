using UnityEditor;
using UnityEngine;

namespace Base.Ravel.Creator.Components.Gizmo
{
	public static class ComponentGizmos
	{
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		private static void OnDrawDefaultGizmo(ComponentBase component, GizmoType type) {
			OnDrawDefaultGizmo(component, component.Icon, type);
		}

		private static void OnDrawDefaultGizmo(ComponentBase component, ComponentBase.GizmoIconType icon, GizmoType type) {
			string name = $"{icon}.png";
			Gizmos.DrawIcon(component.transform.position, name, true);
		}
	}
}