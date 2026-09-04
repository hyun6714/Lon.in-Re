using System;

// 개발 결과 등급
public enum DevelopmentGrade
{
    C = 1,
    B = 2,
    A = 3
}

// 게임 개발시 생성 되는 데이터 결과
[Serializable]
public class GameDevResult
{
    public int gameId; // 게임 식별 ID

    public DevelopmentGrade funGrade;  // 재미

    public DevelopmentGrade graphicGrade;  // 그래픽

    public DevelopmentGrade optimizationGrade; // 최적화
   
    public DevelopmentGrade finalGrade; // 최종 등급
}