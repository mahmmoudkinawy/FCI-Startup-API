namespace API.Extenstions;

public static class DateTimeExtenstions
{
    public static int CalculateAge(this DateTime dob)
    {
        var today = DateTime.Today;

        var age = today.Year - dob.Year;

        if (dob.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
