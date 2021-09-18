using System;

namespace api.Objects.AnyLevel
{
    ///<summary>
    /// used in Branch and Company Tanks Filled
    ///</summary>
    public class TanksFilled
    {
        public string companyId { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
        public string tankId { get; set; }
        public double startVolume { get; set; }
        public double endVolume { get; set; }
        public double filledVolume { get; set; }
        public object manualStartVolume { get; set; }
        public object manualEndVolume { get; set; }
        public double manualFilledVolume { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public bool isFinished { get; set; }
        public string plateNumber { get; set; }
        public string tankName { get; set; }
        public string productName { get; set; }
        public string id { get; set; }
    }
}