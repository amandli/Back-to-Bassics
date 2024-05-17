using UnityEngine;

public class EnemyBattlePawn : BattlePawn, IAttackReceiver
{
    [Header("Enemy References")]
    [SerializeField] private BattleManager _bm;
    [SerializeField] private EnemyStateMachine _esm;
    [SerializeField] private BattleAction[] _battleActions;

    protected override void Awake()
    {
        base.Awake();
        if (Data.GetType() != typeof(EnemyBattlePawnData))
        {
            Debug.LogError("Enemy Battle Pawn is set incorrectly");
            return;
        }
        _bm = FindObjectOfType<BattleManager>();
        _esm = GetComponent<EnemyStateMachine>();

        // Attacks Shouldn't be instantiated, they should come bundled with the enemy prefab!! Its cleaner and more efficient!
        if (_battleActions == null) return;
        foreach (BattleAction action in _battleActions)
        {
            action.ParentPawn = this;
        }
    }
    public void PerformRandomBattleActionSequence()
    {
        BroadcastBattleAction(Random.Range(0, _battleActions.Length), (Direction)Random.Range(0, (int)Direction.None));
    }
    /// <summary>
    /// Select from some attack i to broadcast, and then provide a direction if the attack has variants based on this
    /// </summary>
    /// <param name="i"></param>
    /// <param name="dir"></param>
    public void BroadcastBattleAction(int i, Direction dir)
    {
        _battleActions[i].Broadcast(dir);
    }
    /// <summary>
    /// Select from some attack i to perform, and then provide a direction if the attack has variants based on this
    /// </summary>
    /// <param name="i"></param>
    /// <param name="dir"></param>
    public void PerformBattleAction(int i, Direction dir)
    {
        _battleActions[i].Perform(dir);
    }
    #region IAttackReceiver Methods
    public void ReceiveAttackRequest(IAttackRequester requester)
    {
        if (_esm.IsOnState<EnemyStateMachine.Idle>())
        {
            Damage(requester.AttackDamage());
            Lurch(requester.AttackLurch());
        }
    }

    public void CompleteAttackRequest(IAttackRequester requester)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
