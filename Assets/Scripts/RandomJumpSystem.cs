using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using System;

[BurstCompile]
public partial class RandomJumpSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Create an EntityQuery to get all entities
        EntityQuery entityQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = Array.Empty<ComponentType>() // Match all entities regardless of their components
        });
        // Count entities
        int entityCount = entityQuery.CalculateEntityCount();
        // Log the count
        Debug.Log($"Active Entities Count: {entityCount}");

        //Debug.Log("RandomJumpSystem is running.");
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Initialize a random seed per thread
        uint seed = (uint)((SystemAPI.Time.ElapsedTime + 0.01) * 1000);
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);

        Entities
            .WithBurst()
            .ForEach((ref LocalTransform transform, ref MovementData movement) =>
            {
                //Debug.Log($"Processing entity with position: {transform.Position}");
                // Apply gravity
                movement.CurrentVerticalVelocity -= movement.FallSpeed * deltaTime;

                // Randomly initiate a jump
                if (random.NextFloat(0f, 1f) < 0.01f) // 1% chance per frame to jump
                {
                    movement.CurrentVerticalVelocity = movement.JumpSpeed;
                }

                // Update position
                transform.Position.y += movement.CurrentVerticalVelocity * deltaTime;

                // Reset position if below a certain threshold (e.g., ground level)
                if (transform.Position.y < 0f)
                {
                    transform.Position.y = 0f;
                    movement.CurrentVerticalVelocity = 0f;
                }

                if (transform.Position.y > 5f)
                {
                    transform.Position.y = 5f;
                    movement.CurrentVerticalVelocity = -0.1f;
                }
                
                //Debug.Log($"Entity Position: {transform.Position.y}, Velocity: {movement.CurrentVerticalVelocity}");
            }).ScheduleParallel();
    }
}
