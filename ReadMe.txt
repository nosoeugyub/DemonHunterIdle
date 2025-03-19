[[READ ME]]
프로젝트 코딩 컴벤션입니다.

*Class organization(클레스 내 메소드,변수 위치)

Fields (static/const)
properties
events/delegates (action)
Monobehaviour Methods 
(Awake, Start, OnEnable, OnDisable, OnDestroy,etc.)

Abstract/Virtual/Override Methods
Private Methods
Protected Methods
Public Methods

*네이밍 규칙
전역변수(public,static,const,readonly) 대문자
private, protected 소문자
파라미터(매개변수) _소문자

긴 단어는 조사를 사용하도록 ex. ListToArray/FindOfPlayer (with,over,for,to,of,,,,)
단어에 숫자가 들어가면 숫자키로 (tempVector1,tempVector2..)
잠깐 쓸 변수는 temp_area (지속되는 변수는 일반 소문자로)
전부다 대문자 쓰는 경우는 없음

변수는 명사 단위로
함수는 동사 단위로
리턴값이 bool일 시 Is붙이기
인터페이스 앞엔 I붙이기

*애트리뷰트 위치
애트리뷰트는 엔터로 띄우기
[SerializeField]
private string str;

*함수 형식
괄호는 한 칸 띄어서
void Func() 
{
  //something…
}
함수와 함수 사이는 한 칸 띄우기



*주석
안 쓰는 함수는 주석을 달기
긴 주석은 #region이용하기
특정 함수가 너무 길어지면 #region이용하기

-클래스 신설시 using문에 한줄  띄우고 클래스명 위에 아래 양식 기재
ex)
using문
using문

/// <summary>
/// 작성일자   : 0000-00-00
/// 작성자     : 이름 (OO@gmail.com)
/// 클래스용도 : 클래스 용도 설명 기재
/// </summary>
public class SkillManager : MonoSingleton<SkillManager>


-함수에는 <summary> 로 함수기능 설명 기재
/// <summary>
///함수기능내용 기재
/// </summary>

