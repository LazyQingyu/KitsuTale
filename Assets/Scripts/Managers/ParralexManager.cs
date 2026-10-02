using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ParralexManager : MonoBehaviour
{

    public ParralaxLayer[] layers;
    [SerializeField] private Camera target_camera;

    private readonly List<RuntimeLayer> runtime_layers = new List<RuntimeLayer>();
    private Vector3 previous_camera_position;

    void Start()
    {
        target_camera = target_camera != null ? target_camera : Camera.main;
        if (target_camera == null)
        {
            Debug.LogWarning("ParralexManager needs a camera to scroll its layers.", this);
            return;
        }

        previous_camera_position = target_camera.transform.position;
        if (layers == null)
        {
            return;
        }

        foreach (ParralaxLayer layer in layers)
        {
            CreateLayer(layer);
        }
    }

    void LateUpdate()
    {
        if (target_camera == null)
        {
            return;
        }

        Vector3 camera_delta = target_camera.transform.position - previous_camera_position;
        previous_camera_position = target_camera.transform.position;
        Rect camera_bounds = GetCameraBounds();

        foreach (RuntimeLayer runtime_layer in runtime_layers)
        {
            ParralaxLayer layer = runtime_layer.settings;
            Vector3 layer_delta = new Vector3(
                layer.scroll_x ? camera_delta.x / layer.speed_multiplier : 0f,
                layer.scroll_y ? camera_delta.y / layer.speed_multiplier : 0f,
                0f);

            foreach (SpriteRenderer renderer in runtime_layer.renderers)
            {
                renderer.transform.position += layer_delta;
            }

            RecycleLayer(runtime_layer, camera_bounds, camera_delta);
        }
    }

    void CreateLayer(ParralaxLayer layer)
    {
        if (layer == null || layer.sprite == null || layer.nb_of_copy <= 0)
        {
            return;
        }

        Sprite tile_sprite = Sprite.Create(
            layer.sprite,
            new Rect(0f, 0f, layer.sprite.width, layer.sprite.height),
            new Vector2(0.5f, 0.5f),
            Mathf.Max(0.01f, layer.pixels_per_unit));

        int column_count = layer.scroll_x || !layer.scroll_y
            ? (layer.scroll_y ? Mathf.CeilToInt(Mathf.Sqrt(layer.nb_of_copy)) : layer.nb_of_copy)
            : 1;
        int row_count = layer.scroll_y ? Mathf.CeilToInt((float)layer.nb_of_copy / column_count) : 1;
        float tile_width = tile_sprite.bounds.size.x;
        float tile_height = tile_sprite.bounds.size.y;
        RuntimeLayer runtime_layer = new RuntimeLayer(layer, layer.nb_of_copy);
        Vector3 layer_position = new Vector3(
            target_camera.transform.position.x + layer.base_offset.x,
            target_camera.transform.position.y + layer.base_offset.y,
                transform.position.z);

        for (int index = 0; index < layer.nb_of_copy; index++)
        {
            GameObject tile = new GameObject("Parralax Tile");
            tile.transform.SetParent(transform, false);
            int column = index % column_count;
            int row = index / column_count;
            tile.transform.position = layer_position + new Vector3(column * tile_width, row * tile_height, 0f);

            SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = tile_sprite;
            renderer.sortingOrder = layer.sorting_order;
            runtime_layer.renderers[index] = renderer;
        }

        Vector3 layout_offset = new Vector3(
            (column_count - 1) * tile_width * 0.5f,
            (row_count - 1) * tile_height * 0.5f,
            0f);
        foreach (SpriteRenderer renderer in runtime_layer.renderers)
        {
            renderer.transform.position -= layout_offset;
        }

        runtime_layers.Add(runtime_layer);
    }

    void RecycleLayer(RuntimeLayer runtime_layer, Rect camera_bounds, Vector3 camera_delta)
    {
        ParralaxLayer layer = runtime_layer.settings;
        if (layer.recycle_x && camera_delta.x != 0f)
        {
            if (camera_delta.x > 0f)
            {
                RecycleHorizontal(runtime_layer.renderers, camera_bounds.xMin, true);
            }
            else
            {
                RecycleHorizontal(runtime_layer.renderers, camera_bounds.xMax, false);
            }
        }

        if (layer.recycle_y && camera_delta.y != 0f)
        {
            if (camera_delta.y > 0f)
            {
                RecycleVertical(runtime_layer.renderers, camera_bounds.yMin, true);
            }
            else
            {
                RecycleVertical(runtime_layer.renderers, camera_bounds.yMax, false);
            }
        }
    }

    void RecycleHorizontal(SpriteRenderer[] renderers, float camera_edge, bool moving_right)
    {
        SpriteRenderer edge_renderer = moving_right ? FindLeftmost(renderers) : FindRightmost(renderers);
        while (moving_right && edge_renderer.bounds.max.x < camera_edge)
        {
            SpriteRenderer rightmost = FindRightmost(renderers);
            edge_renderer.transform.position += Vector3.right * (rightmost.bounds.max.x - edge_renderer.bounds.min.x);
            edge_renderer = FindLeftmost(renderers);
        }

        while (!moving_right && edge_renderer.bounds.min.x > camera_edge)
        {
            SpriteRenderer leftmost = FindLeftmost(renderers);
            edge_renderer.transform.position -= Vector3.right * (edge_renderer.bounds.max.x - leftmost.bounds.min.x);
            edge_renderer = FindRightmost(renderers);
        }
    }

    void RecycleVertical(SpriteRenderer[] renderers, float camera_edge, bool moving_up)
    {
        SpriteRenderer edge_renderer = moving_up ? FindBottommost(renderers) : FindTopmost(renderers);
        while (moving_up && edge_renderer.bounds.max.y < camera_edge)
        {
            SpriteRenderer topmost = FindTopmost(renderers);
            edge_renderer.transform.position += Vector3.up * (topmost.bounds.max.y - edge_renderer.bounds.min.y);
            edge_renderer = FindBottommost(renderers);
        }

        while (!moving_up && edge_renderer.bounds.min.y > camera_edge)
        {
            SpriteRenderer bottommost = FindBottommost(renderers);
            edge_renderer.transform.position -= Vector3.up * (edge_renderer.bounds.max.y - bottommost.bounds.min.y);
            edge_renderer = FindTopmost(renderers);
        }
    }

    Rect GetCameraBounds()
    {
        float height = target_camera.orthographicSize * 2f;
        float width = height * target_camera.aspect;
        Vector3 camera_position = target_camera.transform.position;
        return new Rect(camera_position.x - width * 0.5f, camera_position.y - height * 0.5f, width, height);
    }

    static SpriteRenderer FindLeftmost(SpriteRenderer[] renderers)
    {
        SpriteRenderer result = renderers[0];
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.bounds.min.x < result.bounds.min.x) result = renderer;
        }
        return result;
    }

    static SpriteRenderer FindRightmost(SpriteRenderer[] renderers)
    {
        SpriteRenderer result = renderers[0];
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.bounds.max.x > result.bounds.max.x) result = renderer;
        }
        return result;
    }

    static SpriteRenderer FindBottommost(SpriteRenderer[] renderers)
    {
        SpriteRenderer result = renderers[0];
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.bounds.min.y < result.bounds.min.y) result = renderer;
        }
        return result;
    }

    static SpriteRenderer FindTopmost(SpriteRenderer[] renderers)
    {
        SpriteRenderer result = renderers[0];
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.bounds.max.y > result.bounds.max.y) result = renderer;
        }
        return result;
    }

    private sealed class RuntimeLayer
    {
        public readonly ParralaxLayer settings;
        public readonly SpriteRenderer[] renderers;

        public RuntimeLayer(ParralaxLayer settings, int copy_count)
        {
            this.settings = settings;
            renderers = new SpriteRenderer[copy_count];
        }
    }
}

[Serializable]
public class ParralaxLayer
{
    public Texture2D sprite;
    public float speed_multiplier = 1.0f;
    public int nb_of_copy = 2;
    public float pixels_per_unit = 100f;
    public int sorting_order;
    public bool scroll_x = true;
    public bool scroll_y;
    public bool recycle_x = true;
    public bool recycle_y;
    public Vector2 base_offset;
}
