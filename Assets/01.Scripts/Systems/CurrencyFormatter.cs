using UnityEngine;

public static class CurrencyFormatter
{
    private static readonly string[] KoreanUnits = { "", "만", "억", "조", "경", "해" };

    // 한글 표기 : 1,000 미만은 일반 정수, 1만 이상부터 단위 축약
    // 예 : 9,500 => 9,500   /   15,000 => 1.5만   /   200,000,000 => 2억

    public static string Format(double value)
    {
        if (value < 10000)
        {
            return value.ToString("N0"); // 1만 미만은 천 단위 콤마 표기
        }

        int unitIndex = 0;
        while(value >= 10000 && unitIndex < KoreanUnits.Length - 1)
        {
            value /= 10000.0;
            unitIndex++;
        }

        // 포맷 : 정수는 정수로, 소수는 최대 둘째 자리수까지만 (1만, 1.5만, 1.25만)
        return $"{value:0.##}{KoreanUnits[unitIndex]}";
    }
}
