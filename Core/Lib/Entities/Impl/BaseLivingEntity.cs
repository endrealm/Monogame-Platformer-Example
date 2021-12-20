using Core.Lib.Entities.Rendering;

namespace Core.Lib.Entities.Impl
{
    public class BaseLivingEntity<T>: BaseEntity<T>, ILivingEntity where T: BaseLivingEntity<T>
    {
        public BaseLivingEntity(IEntityRenderer<T> renderer) : base(renderer)
        { }
    }
}