﻿using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Reflection;

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

	public static Vector3 GetRandomVector(float multiplier)
	{
		return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0) * multiplier;
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

	public static void PerformCommand(this SortedList<Unit> units, Command command, bool useTier = false)
	{
		if (useTier)
		{
			units.PerformCommand(command, units[0].tier);
		}
		else
		{
			units.PerformCommand(command, 0);
		}
	}

	public static void PerformCommand(this SortedList<Unit> units, Command command, int tier)
    {
		if (tier == 0 || !(command.type.Equals(CommandType.Skill0) || command.type.Equals(CommandType.Skill1) ||
			command.type.Equals(CommandType.Skill2) || command.type.Equals(CommandType.Skill3)))
		{
			foreach (var u in units)
			{
				u.PerformCommand(command);
			}
		}
		else
		{
			foreach (var u in units)
			{
				if (u.tier == tier) u.PerformCommand(command);
			}
		}
    }

    public static float GetBiggestUnitRadius(this SortedList<Unit> units)
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

	public static void RunAfterOneFrame(this MonoBehaviour mono, Action ac)
	{
		mono.StartCoroutine(RunAfterEnum(0, ac));
	}

	public static IEnumerator RunAfterEnum(float sec, Action ac)
    {
		if (sec != 0)
		{
			yield return new WaitForSeconds(sec);
		}
		else
		{
			yield return null;
		}
        ac.Invoke();
    }
}



public static class ReflectionExtensionMethods
{
	public static FieldName GetFieldNameAttribute(this BuffType buffType)
	{
		return Attribute.GetCustomAttribute(typeof(BuffType).GetField(buffType.ToString()), typeof(FieldName)) as FieldName;
	}

	public static void IncreaseFieldValueBy(this Unit unit, BuffType buffType, float value)
	{
		FieldInfo field = unit.GetType().GetField(buffType.GetFieldNameAttribute().name);
		field.SetValue(unit, int.Parse(field.GetValue(unit).ToString()) + value);
	}

	public static void DecreaseFieldValueBy(this Unit unit, BuffType buffType, float value)
	{
		FieldInfo field = unit.GetType().GetField(buffType.GetFieldNameAttribute().name);
		field.SetValue(unit, int.Parse(field.GetValue(unit).ToString()) - value);
	}
}