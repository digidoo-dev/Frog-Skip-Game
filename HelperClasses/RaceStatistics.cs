using UnityEngine;

public class RaceStatistics
{
    public bool RaceFinished { get; private set; }
    public float RaceTime { get; private set; }
    public int FinishedAtPlace { get; private set; }


    public RaceStatistics()
    {
        RaceFinished = false;
        RaceTime = 0;
        FinishedAtPlace = 0;
    }

    public void FinishedRace(float time, int place)
    {
        RaceFinished = true;
        RaceTime = time;
        FinishedAtPlace = place;
    }

}
