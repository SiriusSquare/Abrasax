using UnityEngine;
// T .> ���ø�(c++������ ���ø��̶�� �θ��� ����)
public class NotifyValue<T>
{
    public delegate void ValueChanged(T prev, T next); // ���� ��ȭ �Ҷ����� ����. �����ϰ� �ִ� �ֵ��� ������ ����
    public event ValueChanged OnValueChanged;
    private T _value;
    public T Value
    {
        get
        {
            return _value;
        } // get �ϸ� �ڽ��� value�� ��ȯ
        set
        {
            T before = _value; // �켱������ before�� ���� ������ ��
            _value = value;
            if ((before == null && _value != null) || before.Equals(_value) == false) // ���� ���� �������� ���� ���� �ż���� ��� ������Ʈ�� �� ������ ����
                                                                                      //������ before�� null�̰ų� _value�� null�� �ƴҶ�, �׸��� before�� _value�� ���� �������� �����Ѵ�.
            {
                OnValueChanged?.Invoke(before, value); // true�� ��� before, false�� ��� value�� ��ȯ
            }
        }
    }
    public NotifyValue() // ���� ���ٸ�
    {
        _value = default(T); // �ش��ϴ� Ÿ���� �⺻���� ���
    }
    public NotifyValue(T Value) // ���� ���� �־������
    {
        _value = Value; //���� ���� ���⿡ �־��ش�
    }

}
