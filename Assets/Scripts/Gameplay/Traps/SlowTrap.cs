public class SlowTrap : Trap
{
    public override string TrapType => TrapEnum.TrapType.Slow.ToString(); 
    public override float Duration => 5f;
}
