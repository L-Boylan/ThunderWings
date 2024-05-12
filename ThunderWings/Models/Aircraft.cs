namespace ThunderWings.Models;

public class Aircraft
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? Country { get; set; }
    public string? Role { get; set; }
    public int TopSpeed { get; set; }
    public int Price { get; set; }
}