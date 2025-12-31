// MockUserData.cs
using server.Models;
using Microsoft.AspNetCore.Identity;

public static class MockUserData
{
    public static ApplicationUser User = new ApplicationUser
    {
        Id = 1,
        FullName = "abc",
        Email = "r.rhm@gmail.com",
        UserName = "r.rhm@gmail.com",
        PasswordHash = "AQAAAAIAAYagAAAAEG0luV9tYiu6rB/3VTb8iqRmV2Ee+zgwgvmc9AsMZWVPEXiU7tvODG2UE4c8dwDykA=="
    };

    public static string PlainPassword = "Dat@1912";
    public static string Role = "Doctor";
}
