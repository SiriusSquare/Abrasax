using UnityEngine;
// T .> 템플릿(c++에서는 템플릿이라고 부르기 떄문)
public class NotifyValue<T>
{
    public delegate void ValueChanged(T prev, T next); // 값이 변화 할때마다 통지. 구독하고 있는 애들이 통지를 받음
    public event ValueChanged OnValueChanged;
    private T _value;
    public T Value
    {
        get
        {
            return _value;
        } // get 하면 자신의 value를 반환
        set
        {
            T before = _value; // 우선적으로 before에 값을 저장한 후
            _value = value;
            if ((before == null && _value != null) || before.Equals(_value) == false) // 값이 같지 않을때만 통지 이퀄 매서드는 모든 오브젝트가 다 가지고 있음
                                                                                      //만ㅇ갸 before가 null이거나 _value가 null이 아닐때, 그리고 before와 _value가 같지 않을때만 통지한다.
            {
                OnValueChanged?.Invoke(before, value); // true일 경우 before, false일 경우 value를 반환
            }
        }
    }
    public NotifyValue() // 값이 없다면
    {
        _value = default(T); // 해당하는 타입의 기본값을 뱉기
    }
    public NotifyValue(T Value) // 만약 식을 넣었을경우
    {
        _value = Value; //받은 값을 여기에 넣어준다
    }

}
