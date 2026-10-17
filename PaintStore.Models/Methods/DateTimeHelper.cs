namespace PaintStore.Models.Methods;

// a static class is a class that can be used without creating an instance of it

/*
more detailed explanation of static class:
a static class is a class that can be used without creating an instance of it.
when you create an instance of a static class, you are actually creating an instance of the class, 
but the instance is not stored in memory.
what does it mean?
It means that you can call methods on a static class without creating an instance of it.
For example:
DateTimeHelper.GetNow();
This will return the current date and time, without creating an instance of the DateTime class.

*/

public static class DateTimeHelper
{
    public static DateTime GetCurrentDateTime() {
        return DateTime.Now;
    }

    public static DateTime GetCurrentDate() {
        return DateTime.Today;
    }


    public static int DaysBetween(DateTime start, DateTime end) {
        return (end-start).Days;
    }

    public static bool isWeekend(DateTime date) {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    public static DateTime AddDays(DateTime date, int days) {
        return date.AddDays(days);
    }




}
