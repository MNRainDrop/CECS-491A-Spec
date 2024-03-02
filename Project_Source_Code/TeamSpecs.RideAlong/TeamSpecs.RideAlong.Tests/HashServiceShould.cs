namespace TeamSpecs.RideAlong.Tests;
using RideAlong.Services.HashService;
using System.Diagnostics.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;

public class HashingServiceShould
{
    [Fact]
    public void CreateTwoMatchingHashesFromTheSameInformation()
    {
        //Arrange
        int salt_1 = 01;
        int salt_2 = 01;
        int pepper_1 = 02;
        int pepper_2 = 02;
        string string_1 = "Test String";
        string string_2 = "Test String";
        var hashService_1 = new HashService();
        var hashService_2 = new HashService();
        var expectedValue = true;
        var actualValue = false;
        string hash1 = string.Empty;
        string hash2 = string.Empty;

        //Act
        try
        {
            hash1 = hashService_1.hashPass(salt_1, pepper_1, string_1);   
            hash2 = hashService_2.hashPass(salt_2, pepper_2, string_2);
            if (hash1 == hash2)
            {
                actualValue = true;
            }
        } catch
        {
            actualValue = false;
        }
        //Assert
        Assert.Equal(expectedValue, actualValue);
        Console.WriteLine($"{hash1}\n{hash2}");
    }
    [Fact]
    public void CreateTwoHashesFromDifferentSalt()
    {
        //Arrange
        int salt_1 = 1;
        int salt_2 = 2;
        int pepper = 3;
        string text = "Test String";
        var hashService = new HashService();
        var expectedValue = true;
        var actualValue = false;
        string hash1 = string.Empty;
        string hash2 = string.Empty;
        //Act
        try
        {
            hash1 = hashService.hashPass(salt_1, pepper, text);
            hash2 = hashService.hashPass(salt_2, pepper, text);
            if (hash1 != hash2)
            {
                actualValue = true;
            }
        }
        catch
        {
            actualValue = false;
        }
        //Assert
        Assert.Equal(expectedValue, actualValue);
        Console.WriteLine($"{hash1}\n{hash2}");
    }
    [Fact]
    public void CreateTwoHashesFromDifferentStrings()
    {
        //Arrange
        int salt = 1;
        int pepper = 2;
        string string_1 = "Test String";
        string string_2 = "String Test";
        var hashService = new HashService();
        var expectedValue = true;
        var actualValue = false;
        string hash1 = string.Empty;
        string hash2 = string.Empty;
        //Act
        try
        {
            hash1 = hashService.hashPass(salt, pepper, string_1);
            hash2 = hashService.hashPass(salt, pepper, string_2);
            if (hash1 != hash2)
            {
                actualValue = true;
            }
        }
        catch
        {
            actualValue = false;
        }
        //Assert
        Console.WriteLine($"{hash1}\n{hash2}");
        Assert.Equal(expectedValue, actualValue);
    }
}