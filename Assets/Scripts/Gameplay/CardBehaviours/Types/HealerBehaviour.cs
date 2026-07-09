using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 힐러 — 매 턴 시작 시 다른 전방 아군 회복. 데이터는 베이스 직렬화 필드, 회복 튜닝은 아래 인스펙터 필드에서 편집한다.
[CreateAssetMenu(fileName = "HealerBehaviour", menuName = "Scriptable Object/Card Behaviour/Healer")]
public class HealerBehaviour : CardBehaviour
{
    [CenterHeader("< 힐러 튜닝 >")]
    [SerializeField] private int _frontTicks = 1; // 전방 힐러의 회복 횟수
    [SerializeField] private int _backTicks  = 3; // 후방 힐러의 회복 횟수
    [SerializeField] private int _healAmount = 1; // 1틱당 회복량

    // 회복 가능한 다른 전방 아군을 1씩 여러 틱 회복한다. 틱마다 텀을 둬 +N 팝업이 겹치지 않게 한다
    public override IEnumerator OnTurnStartPassive(TurnPassiveContext ctx)
    {
        int ticks = ctx.IsFront ? _frontTicks : _backTicks;

        for (int tick = 0; tick < ticks; tick++)
        {
            Entity target = PickHealTarget(ctx.Self, ctx.IsMine);
            if (target == null) // 회복할 대상이 없으면 조기 종료
            {
                yield break;
            }

            target.Heal(_healAmount);

            yield return ctx.Delay;
        }
    }

    // 자신 제외, 회복 가능한 전방 아군 무작위 1명 (없으면 null)
    private static Entity PickHealTarget(Entity healer, bool isMine)
    {
        var candidates = new List<Entity>();
        foreach (Entity entity in Services.Get<EntityManager>().GetFront(isMine))
        {
            if (entity != healer && entity.CanHeal)
            {
                candidates.Add(entity);
            }
        }

        return candidates.Count == 0 ? null : candidates[Random.Range(0, candidates.Count)];
    }
}
