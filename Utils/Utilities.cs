namespace GymProApi.Utils
{
    public static class Utilities
    {
        public static string ToSqlDateFormat(this DateOnly? value)
        {
            return (value.HasValue && value is not null) ? value.Value.ToString("yyyy-MM-dd") : "null";
        }
    }
}
