using Microsoft.AspNetCore.Mvc;
using backend.Records;

namespace backend.Controllers;

[ApiController]
[Route("/exposure/noise")]
public class NoiseDataController
{
    public IEnumerable<NoiseData> Get()
    {
        Random random = new Random();
        List<NoiseData> data = new List<NoiseData>();
        for (int i = 0; i < 10; i++)
        {
            data.Add(new NoiseData(random.Next(50, 100), new TimeOnly()));
        }
        return data;
    }
}