namespace TP4.Models;

public class Triangle : Shape
{
    public int GetSideLength()
    {
        return Size switch
        {
            "small" => 100,
            "large" => 300,
            _ => 200
        };
    }
}