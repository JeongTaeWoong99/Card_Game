using System.Collections.Generic;

// ECardType → 행동 SO 매핑 파사드. 레지스트리 SO로 초기화되며, 호출부는 Of(type)만 사용한다 (흩어진 switch 제거).
// 새 카드 타입은 behaviour SO를 만들어 레지스트리에 추가하는 것으로 끝난다 (코드 무수정).
public static class CardBehaviours
{
    private static Dictionary<ECardType, ICardBehaviour> _map;

    // 레지스트리 SO로 매핑을 구성한다 (CardManager.Awake가 모든 Start 이전에 호출)
    public static void Init(CardBehaviourRegistrySO registry)
    {
        _map = registry.BuildMap();
    }

    // 해당 타입의 행동 SO를 반환한다 (미초기화/미등록 시 예외로 초기화 순서·할당 누락을 즉시 드러낸다)
    public static ICardBehaviour Of(ECardType type)
    {
        return _map[type];
    }
}
