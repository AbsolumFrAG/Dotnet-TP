using System.Text;
using Microsoft.AspNetCore.Mvc;
using TP4.Models;

namespace TP4.Controllers;

public class ShapesController : Controller
{
    public IActionResult Circle()
    {
        var circle = new Circle();
        return View(circle);
    }

    [HttpPost]
    public IActionResult Circle(Circle circle)
    {
        return View(circle);
    }

    public IActionResult Triangle()
    {
        var triangle = new Triangle();
        return View(triangle);
    }

    [HttpPost]
    public IActionResult Triangle(Triangle triangle)
    {
        return View(triangle);
    }

    public IActionResult Square()
    {
        var square = new Square();
        return View(square);
    }

    [HttpPost]
    public IActionResult Square(Square square)
    {
        return View(square);
    }
}