namespace TP4.Models;

public class Circle : Shape
{
    public int GetRadius()
    {
        return Size switch
        {
            "small" => 50,
            "large" => 150,
            _ => 100
        };
    }
}