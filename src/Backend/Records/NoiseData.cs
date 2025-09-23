using System.ComponentModel.DataAnnotations;

namespace Backend.Records;

public record NoiseData([property: Key] Guid Id, double LavgQ3, TimeOnly Time);