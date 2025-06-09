namespace jamesmvc.Models
{
    public class LogisticsTrackingViewModel
    {
        public LogisticsOrder Order { get; set; }
        public List<TrackingRecord> TrackingRecords { get; set; }
    }
}
