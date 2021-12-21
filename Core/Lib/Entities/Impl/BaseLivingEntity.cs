using Core.Lib.Entities.Rendering;

namespace Core.Lib.Entities.Impl
{
    public class BaseLivingEntity<T>: BaseEntity<T>, ILivingEntity where T: BaseLivingEntity<T>
    {
        private readonly int _maxHealth;
        private int _currentHealth;

        public BaseLivingEntity(IEntityRenderer<T> renderer, int maxHealth) : base(renderer)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        /// <summary>
        /// Will call revive in case hp should be reset
        /// </summary>
        /// <returns>if true entity is removed</returns>
        public virtual bool Die()
        {
            return false;
        }
        
        public void FullHeal()
        {
            _currentHealth = _maxHealth;
        }
        public void GetDamage(IDamageSource source, DamageReason damageReason, int damage)
        {
            _currentHealth -= damage;
            
            if(_currentHealth > 0) return;

            if (!Die())
            {
                return;
            }
            
            // TODO destroy entity here
        }
    }
}