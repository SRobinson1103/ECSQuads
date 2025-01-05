using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public partial class TerrainGenerationSystem : SystemBase
{
    public TerrainSettings Settings; // Reference to TerrainSettings

    private Mesh squareMesh;
    private Material tileMaterial;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        // Find the TerrainReferences component in the scene
        TerrainReferences terrainReferences = GameObject.FindFirstObjectByType<TerrainReferences>();
        if (terrainReferences == null)
        {
            Debug.LogError("TerrainReferences not found! Ensure a GameObject with the TerrainReferences script is present in the scene.");
            return;
        }

        // Assign the mesh and material from TerrainReferences
        squareMesh = terrainReferences.SquareMesh;
        tileMaterial = terrainReferences.TileMaterial;
        Settings = terrainReferences.terrainSettings;

        if (squareMesh == null || tileMaterial == null || Settings == null)
        {
            Debug.LogError("SquareMesh, TileMaterial or TerrainSettings is not assigned in TerrainReferences!");
            return;
        }

        // Generate the terrain
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        int gridWidth = Settings.GridWidth;
        int gridHeight = Settings.GridHeight;
        float tileSize = Settings.TileSize;

        // Create a RenderMeshArray with the assigned mesh and material
        RenderMeshArray renderMeshArray = new RenderMeshArray(
            new Material[] { tileMaterial },
            new Mesh[] { squareMesh }
        );

        // Create a MaterialMeshInfo referencing the first (and only) mesh and material in the array
        MaterialMeshInfo materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);

        // Create a RenderMeshDescription
        RenderMeshDescription renderMeshDescription = new RenderMeshDescription(
            shadowCastingMode: UnityEngine.Rendering.ShadowCastingMode.Off,
            receiveShadows: true
        );

        // Create a prefab entity with the necessary components
        EntityArchetype archetype = EntityManager.CreateArchetype(
            typeof(LocalToWorld),
            typeof(LocalTransform),
            typeof(RenderBounds),
            typeof(MaterialMeshInfo),
            typeof(URPMaterialPropertyBaseColor), // Adds support for per-entity colors
            typeof(MovementData) // Add MovementData to the archetype
        );
        Entity tilePrefab = EntityManager.CreateEntity(archetype);

        // Add rendering components to the prefab entity
        RenderMeshUtility.AddComponents(
            tilePrefab,
            EntityManager,
            renderMeshDescription,
            renderMeshArray,
            materialMeshInfo
        );

        // Generate the grid of tiles
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Entity tile = EntityManager.Instantiate(tilePrefab);
                //Debug.Log($"Entity created with LocalToWorld and MovementData components: {tile}");

                float3 position = new float3(x * tileSize, 0, z * tileSize);
                EntityManager.SetComponentData(tile, new LocalToWorld
                {
                    Value = float4x4.TRS(position, quaternion.identity, new float3(1, 1, 1))
                });

                // Initialize MovementData
                EntityManager.SetComponentData(tile, new MovementData
                {
                    JumpSpeed = Settings.JumpSpeed,
                    FallSpeed = Settings.FallSpeed,
                    CurrentVerticalVelocity = 0f
                });

                EntityManager.SetComponentData(tile, new LocalTransform
                {
                    Position = new float3(x * tileSize, 0, z * tileSize),
                    Rotation = quaternion.identity,
                    Scale = 1.0f // Ensure scale is initialized
                });

                // Assign default color
                EntityManager.SetComponentData(tile, new URPMaterialPropertyBaseColor
                {
                    //Value = new float4(Settings.DefaultColor.r, Settings.DefaultColor.g, Settings.DefaultColor.b, Settings.DefaultColor.a)
                    Value = new float4(UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f))
                });
            }
        }

        // Clean up the prefab entity
        EntityManager.DestroyEntity(tilePrefab);
    }

    protected override void OnUpdate()
    {
        // No update logic required for this system
    }
}
