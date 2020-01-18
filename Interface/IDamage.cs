namespace Game
{
    public interface IDamage
    {
        float Damage(float hp);
        float SpecialDamage(float hp);
        bool StunDamage();
    }
}
