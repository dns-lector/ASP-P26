namespace ASP_P26.Services.Time
{
    public class MillisecTimeService : ITimeService
    {
        public long Timestamp()
        {
            return (DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / (long)1e4;
        }
    }
}
