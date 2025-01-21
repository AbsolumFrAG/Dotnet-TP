using TP3.Models;

namespace TestTP3;

public class BookModelTests
{
    [Fact]
    public void Book_CreatesValidInstance()
    {
        // Arrange & Act
        var book = new Book
        {
            Id = 1,
            Title = "Test Title",
            Author = "Test Author"
        };

        // Assert
        Assert.Equal(1, book.Id);
        Assert.Equal("Test Title", book.Title);
        Assert.Equal("Test Author", book.Author);
    }
}