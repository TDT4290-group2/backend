using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Backend.DTOs;
public record CreateNoiseDataDto(
    [Required] double LavgQ3,
    [Required] DateTime Time
);