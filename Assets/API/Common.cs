﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Common
{
    public static GameObject GetObjectUnderMouse(bool disableWarning = false)
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
        {
            return hit.collider.gameObject;
        }

		if (!disableWarning)
		{
			Debug.LogWarning("There is no object under the mouse.");
		}
        return null;
    }

    public static Vector3 GetWorldMousePoint(LayerMask layerMask)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, layerMask))
        {
            return hit.point;
        }

        Debug.LogWarning("Did not clicked on the layer.");
        return Vector3.zero;
    }

    public static float GetRawDistance2D(Vector3 vec, Vector3 other)
    {
        var x = vec.x - other.x;
        var z = vec.z - other.z;
        return x * x + z * z;
    }
}



public static class ExtensionMethods
{
    public static bool Contains(this RectTransform[] rectTransforms, Vector3 point)
    {
        foreach (var rectTrans in rectTransforms)
        {
            if (rectTrans.GetRect().Contains(point))
            {
                return true;
            }
        }
        return false;
    }

    public static Rect GetRect(this RectTransform rectTransform)
    {
        return new Rect(rectTransform.GetScreenPosition(), rectTransform.rect.size);
    }

    public static Vector2 GetScreenPosition(this RectTransform rectTransform)
    {
        return rectTransform.position - Vector3.Scale(rectTransform.rect.size, rectTransform.pivot);
    }

    public static void PerformCommand(this List<Unit> units, Command command)
    {/*
		if (units.Count > 1 && (command.Equals(Command.Move) || (command.Equals(Command.Attack) && unit == null)))
		{
			int cols = Mathf.RoundToInt(Mathf.Sqrt(units.Count));

			//TODO: construct a grid of units
		}
		else
		{*/
        foreach (var u in units)
        {
            u.PerformCommand(command);
        }
        //}
    }

    public static float GetBiggestUnitRadius(this List<Unit> units)
    {
        float max = 0;
        foreach (var u in units)
        {
            max = (max > u.radius) ? max : u.radius;
        }
        return max;
    }

    public static Vector3 GetObjectSize(this GameObject go)
    {
        Vector3 size = new Vector3(1, 1, 1);
        //First look for SkinnedMeshRenderer
        SkinnedMeshRenderer[] meshes = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (meshes.Length > 0)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (SkinnedMeshRenderer r in meshes)
            {
                bounds.Encapsulate(r.sharedMesh.bounds);
            }
            size = bounds.size;
        }
        //If no skinned, look for mesh filters
        else
        {
            MeshFilter[] meshesf = go.GetComponentsInChildren<MeshFilter>();
            if (meshesf.Length > 0)
            {
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach (MeshFilter r in meshesf)
                {
                    bounds.Encapsulate(r.mesh.bounds);
                }
                size = bounds.size;
            }
        }
        return size;
    }

    public static void Log<T>(this Unit u, T t)
    {
        if (u.team.Equals(Team.T1))
        {
            Debug.Log(t.ToString());
        }
    }

    public static void RunAfter(this MonoBehaviour mono, float sec, Action ac)
    {
        mono.StartCoroutine(RunAfterEnum(sec, ac));
    }

    public static IEnumerator RunAfterEnum(float sec, Action ac)
    {
        yield return new WaitForSeconds(sec);
        ac.Invoke();
    }
}
