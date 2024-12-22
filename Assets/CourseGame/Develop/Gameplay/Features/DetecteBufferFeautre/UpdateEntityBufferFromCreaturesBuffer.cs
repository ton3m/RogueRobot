using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Gameplay.Entities.Behaviours;
using System.Collections.Generic;

namespace Assets.CourseGame.Develop.Gameplay.Features.DetecteBufferFeautre
{
    public class UpdateEntityBufferFromCreaturesBuffer : IEntityInitialize, IEntityUpdate
    {
        private EntitiesBuffer _creaturesBuffer;

        private List<Entity> _detectedEntities;

        public UpdateEntityBufferFromCreaturesBuffer(EntitiesBuffer creaturesBuffer)
        {
            _creaturesBuffer = creaturesBuffer;
        }

        public void OnInit(Entity entity)
        {
            _detectedEntities = entity.GetDetectedEntitiesBuffer();
        }

        public void OnUpdate(float deltaTime)
        {
            _detectedEntities.Clear();

            _detectedEntities.AddRange(_creaturesBuffer.Elements);
        }
    }
}
