using UnityEngine;

public interface Istate
{
    void Enter(); // 상태에 진입할 때 호출
    void Execute(Vector2 playerVector); // 상태가 활성화 되어 있을때 매 프레임 호출
    void Exit(); // 상태에서 벗어날때 호출
}