using System;
namespace MeeeApp.Extensions
{
	public static class DateExtensions
	{
		public enum PartOfDay
		{
			Morning,
			Afternoon,
			Evening
		}

		public static PartOfDay GetPartOfDay()
		{
			var now = DateTime.Now;
			if (now.Hour >= 0 && now.Hour < 12)
			{
				return PartOfDay.Morning;
			}
			else if (now.Hour >= 12 && now.Hour < 17)
			{
				return PartOfDay.Afternoon;
			}
			else
			{
				return PartOfDay.Evening;
			}
		}
	}
}

