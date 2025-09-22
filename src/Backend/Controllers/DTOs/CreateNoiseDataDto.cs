using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace backend.DTOs;
public record CreateNoiseDataDto(
    [Required] double LavgQ3,
    [Required] TimeOnly Time
);