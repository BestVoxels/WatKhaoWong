using System;

namespace WatKhaoWong.Retreats
{
    public class SetTimeData
    {
        public EIsStaying isStayingOvernight;
        public DateTime startDate;
        public DateTime endDate;
        public TimeSpan duration;



        #region --Methods-- (Custom PUBLIC)
        public ETimePeriod? GetTimePeriod(DateTime nowDate)
        {
            if (startDate == default)
                return null;

            if (startDate.Date < nowDate.Date)
                return ETimePeriod.Past;

            if (startDate.Date == nowDate.Date)
                return ETimePeriod.Present;

            if (startDate.Date > nowDate.Date)
                return ETimePeriod.Future;

            return null;
        }
        #endregion
    }
}