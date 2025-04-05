namespace TouristServer.Presentation.Dto;

public class FilterDto
{
    public List<Guid>? CategoryIds { get; set; } = [];
    public List<Guid>? DistrictIds { get; set; } = [];
    public long? ConstMin { get; set; }
    public long? ConstMax { get; set; }
}