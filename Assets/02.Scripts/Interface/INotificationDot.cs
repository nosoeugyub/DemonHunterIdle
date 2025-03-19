/// 작성일자   : 2024-05-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 알림인터페이스
/// </summary>

public interface INotificationDot<T>
{
    void ShowDot(T runType);
    void HideDot(T runType);
}