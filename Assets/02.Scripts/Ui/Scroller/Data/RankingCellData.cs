using System;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class RankingCellData : IComparer<RankingCellData>, IComparable<RankingCellData>
{
    public Utill_Enum.RankType rankType;
    public int rank;
    public float percent;
    public long rankValue;
    public int extraValue; //부가적으로 표기해야될 값 현재는 레벨을 담는 변수로 이용
    public string id;
    public string country;
    public DateTime date;

    public int Compare(RankingCellData x, RankingCellData y)
    {
        if (x.rank == y.rank)
            return x.date.CompareTo(y.date);
        else
            return x.rank.CompareTo(y.rank);
    }

    public int CompareTo(RankingCellData other)
    {
        return Compare(this, other);
    }
}