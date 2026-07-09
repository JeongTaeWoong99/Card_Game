using UnityEngine;

// 상대 진영 구현 종류. ComputerAI = 룰 기반 AI(EnemyAI), TestPlayer = 서버 연동 흉내(원격 사람).
public enum EOpponentType
{
    ComputerAI,
    TestPlayer,
}

// 게임 시작 시 선택된 상대 GameObject만 활성화한다. 활성화된 쪽의 Awake가 자신을 IEnemyAI로
// 등록하고(자기 등록형 서비스 로케이터), 나머지는 비활성이라 등록되지 않는다 → 결정적 교체.
// TurnManager 등 호출부는 Services.Get<IEnemyAI>()만 쓰므로 어느 구현이 등록됐는지 알 필요가 없다 (DIP).
public class OpponentSelector : MonoBehaviour
{
    [CenterHeader("< 상대 선택 >")]
    [SerializeField] private EOpponentType _selected = EOpponentType.ComputerAI;

    [CenterHeader("< 대상 (둘 다 비활성으로 시작) >")]
    [SerializeField] private GameObject _computerAI; // EnemyAI 오브젝트
    [SerializeField] private GameObject _testPlayer; // TestPlayer 오브젝트

    // 선택된 상대만 활성화해 IEnemyAI로 등록시킨다 (GameManager.StartGame이 게임 사용 전에 호출)
    public void ApplySelection()
    {
        bool useTest = _selected == EOpponentType.TestPlayer;

        _computerAI.SetActive(!useTest); // 활성화되는 순간 Awake → Register<IEnemyAI>
        _testPlayer.SetActive(useTest);

        Debug.Log($"[OpponentSelector] 상대 = {_selected}");
    }
}
