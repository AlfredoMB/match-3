public class GameTimer
{
    public float RemainingTime { get; private set; }
    public bool TimeIsUp { get; private set; }

    public void SetTime(float time)
    {
        RemainingTime = time;
    }

    public void UpdateTimePassed(float timePassed)
    {
        RemainingTime -= timePassed;
        if (RemainingTime <= 0)
        {
            RemainingTime = 0;
            TimeIsUp = true;
        }
    }
}
