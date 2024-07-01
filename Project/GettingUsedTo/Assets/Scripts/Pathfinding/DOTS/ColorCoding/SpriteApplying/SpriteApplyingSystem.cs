using Pathfinding.DOTS.ColorCoding.Sprites;
using Pathfinding.DOTS.RandomSystem;
using Unity.Entities;
using UnityEngine;

namespace Pathfinding.DOTS.ColorCoding.SpriteApplying
{
    public partial struct SpriteApplyingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ColorSpritesData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            return;
            var spritesData = SystemAPI.ManagedAPI.GetSingleton<ColorSpritesData>();

            foreach (var (spriteRenderer, pathData, randomData, hasSpriteApplied) in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<SpriteRenderer>, RefRO<PathData>, RefRW<IndividualRandomData>, EnabledRefRW<HasSpriteApplied>>().WithAll<HasColorDefined>().WithDisabled<HasSpriteApplied>())
            {
                spriteRenderer.Value.sprite = spritesData.sprites[GetRandomIndexFrom2DArray(pathData.ValueRO.colorIndex,spritesData.sprites.Length, ref randomData.ValueRW)];
                hasSpriteApplied.ValueRW = true;
            }
            
            int GetRandomIndexFrom2DArray(int x, int arrayLength, ref IndividualRandomData randomData)
            {
                var randomIndex = randomData.spriteRandom.NextInt(arrayLength);
                return x + randomIndex - (randomIndex % 3);
            }
        }
    }
}