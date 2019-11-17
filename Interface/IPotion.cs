namespace Game
{
    public interface IPotion
    {
        void TakePotion(int id);
        void HealthPotion(int id, float heal);
        void EnergyPotion(int id, float energy, float cd, float time);
        void PowerPotion(int id, float dmg, float time);
    }
}
