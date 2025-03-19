
/// <summary>
/// 전략 패턴 제네릭 인터페이스
/// </summary>
/// <typeparam name="T">매개변수</typeparam>
/// <typeparam name="TResult">반환값</typeparam>
public interface IStrategy<T,TResult>
{
    TResult DoAlgorithm(T data);
}
/// <summary>
/// 반환값 없는 전략 패턴 제네릭 인터페이스
/// </summary>
/// <typeparam name="T">매개변수</typeparam>
public interface IStrategy<T>
{
    void DoAlgorithm(T data);
}