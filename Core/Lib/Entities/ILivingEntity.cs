namespace Core.Lib.Entities
{
    public interface ILivingEntity: IEntity
    {
        void GetDamage(IDamageSource source, DamageReason damageReason, int damage);
    }
}