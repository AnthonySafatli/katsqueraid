public class StunTrap : Trap
{
    public override string TrapType => TrapEnum.TrapType.Stun.ToString(); 
    public override float Duration => 3f;
}
