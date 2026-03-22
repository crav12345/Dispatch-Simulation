using System.Collections.Generic;
using UnityEngine;

public static class CitySceneBuilder
{
    private const float DefaultRoadWidth = 0.25f;
    private const float DefaultIntersectionScale = 0.9f;
    private const float DefaultRoadDepth = 0.05f;
    private const float DefaultIntersectionDepth = 0.35f;

    public static GameObject Build(City city, Transform parent = null)
    {
        var root = new GameObject("City");

        if (parent != null)
        {
            root.transform.SetParent(parent, false);
        }

        var roadsRoot = new GameObject("Roads");
        roadsRoot.transform.SetParent(root.transform, false);

        var intersectionsRoot = new GameObject("Intersections");
        intersectionsRoot.transform.SetParent(root.transform, false);

        var renderedRoads = new HashSet<long>();
        var roadMaterial = CreateRoadMaterial();

        foreach (var road in city.Roads)
        {
            var key = GetConnectionKey(road.FromIdx, road.ToIdx);

            if (!renderedRoads.Add(key))
            {
                continue;
            }

            var from = city.Intersections[road.FromIdx];
            var to = city.Intersections[road.ToIdx];
            CreateRoadObject(roadsRoot.transform, roadMaterial, from.Position, to.Position);
        }

        for (var i = 0; i < city.Intersections.Count; i++)
        {
            CreateIntersectionObject(intersectionsRoot.transform, city.Intersections[i], i);
        }

        return root;
    }

    private static long GetConnectionKey(int firstID, int secondID)
    {
        var min = Mathf.Min(firstID, secondID);
        var max = Mathf.Max(firstID, secondID);
        return ((long)min << 32) | (uint)max;
    }

    private static Material CreateRoadMaterial()
    {
        var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Standard");
        var material = new Material(shader);
        material.color = new Color(0.12f, 0.12f, 0.12f);
        return material;
    }

    private static void CreateRoadObject(Transform parent, Material material, Vector2 from, Vector2 to)
    {
        var roadObject = new GameObject($"Road_{from}_{to}");
        roadObject.transform.SetParent(parent, false);

        var lineRenderer = roadObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = DefaultRoadWidth;
        lineRenderer.endWidth = DefaultRoadWidth;
        lineRenderer.numCapVertices = 6;
        lineRenderer.SetPosition(0, ToRoadWorldPosition(from));
        lineRenderer.SetPosition(1, ToRoadWorldPosition(to));
    }

    private static void CreateIntersectionObject(Transform parent, Intersection intersection, int intersectionIndex)
    {
        var intersectionObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        intersectionObject.name = $"Intersection_{intersectionIndex}";
        intersectionObject.transform.SetParent(parent, false);
        intersectionObject.transform.localPosition = ToWorldPosition(intersection.Position);
        intersectionObject.transform.localScale = Vector3.one * DefaultIntersectionScale;

        var renderer = intersectionObject.GetComponent<Renderer>();
        renderer.material.color = new Color(0.82f, 0.22f, 0.2f);
    }

    private static Vector3 ToWorldPosition(Vector2 point)
    {
        return new Vector3(point.x, point.y, DefaultIntersectionDepth);
    }

    private static Vector3 ToRoadWorldPosition(Vector2 point)
    {
        return new Vector3(point.x, point.y, DefaultRoadDepth);
    }
}
