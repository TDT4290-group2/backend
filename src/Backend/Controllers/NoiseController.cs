using Backend.DTOs;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;

[GraphRoute("noise")]
public class NoiseController : GraphController
{

    [Query("noises")]
    public NoiseDataResponseDto[] GetNoises()
    {
        // Dummy data for demonstration purposes
        var noises = new[]
        {
            new NoiseDataResponseDto(Guid.NewGuid(), 65.5, DateTime.UtcNow),
            new NoiseDataResponseDto(Guid.NewGuid(), 70.2, DateTime.UtcNow.AddMinutes(-10)),
        };
        return noises;
    }

    [Mutation("addNoise")]
    public NoiseDataResponseDto AddNoise(double decibels)
    {
        var newNoise = new NoiseDataResponseDto(Guid.NewGuid(), decibels,
        DateTime.UtcNow);
        return newNoise;
    }
}