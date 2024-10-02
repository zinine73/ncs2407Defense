using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate; // 타워 정보
    [SerializeField] private GameObject towerPrefab; // 타워 프리펩 연결
    [SerializeField] private InfoTower infoTower; // 타워정보 패널
    private ContactFilter2D filter; // Raycast용 파라미터
    private List<RaycastHit2D> rcList; // Raycast 결과 저장용 리스트

    private void Start()
    {
        filter = new ContactFilter2D(); // 파라미터 생성
        rcList = new List<RaycastHit2D>(); // 리스트 생성    
    }

    private void Update()
    {
        // 마우스가 UI 에 있을 때는 바로 리턴
        if (EventSystem.current.IsPointerOverGameObject() == true) return;
        
        // 마우스 왼쪽 버튼 클릭하면
        if (Input.GetMouseButtonDown(0))
        {
            // 리스트를 클리어 하고
            rcList.Clear();
            // 월드포지션값을 구해서
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // ray를 쏴서 검출되는 오브젝트들 찾기
            Physics2D.Raycast(worldPosition, Vector2.zero, filter, rcList);
            // 리스트를 돌면서
            foreach (var item in rcList)
            {
                // "TOWER" 태그인 아이템이 있으면
                if (item.transform.CompareTag("TOWER"))
                {
                    // 타워 정보 패널에 표시할 정보를 넘기고 패널 켜기
                    infoTower.OnPanel(item.transform);
                    // 타워가 이미 있으므로 리턴
                    return;
                }
            }
            // 다시한번 리스트를 돌면서
            foreach (var item in rcList)
            { 
                // "TILE" 태그인 아이템이 있으면
                if (item.transform.CompareTag("TILE"))
                {
                    // 그 자리에 타워 생성
                    SpawnTower(item.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) // 다른곳 클릭했을 때 패널정보 없애기
        {
            foreach (var item in rcList)
            {
                // 타워가 있는 곳은 빼고
                if (item.transform.CompareTag("TOWER"))
                {
                    return;
                }
            }
            // 아닌 곳에서는 정보 패널 끄기
            infoTower.OffPanel();
        }
    }

    private void SpawnTower(Transform tileTransform)
    {
        // 건설 소요 비용이 소지 골드보다 크면 리턴
        if (towerTemplate.weapon[0].cost > PlayerManager.instance.CurrentGold)
        {
            // todo 건설 불가 메시지 출력

            return;
        }
        // 소지골드에서 건설비용 차감
        PlayerManager.instance.CurrentGold -= towerTemplate.weapon[0].cost;
        // 타워프리펩으로 타워 생성
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, 
            Quaternion.identity, transform);
        // 타워 무기 초기화
        clone.GetComponent<TowerWeapon>().Init();
    }
}
