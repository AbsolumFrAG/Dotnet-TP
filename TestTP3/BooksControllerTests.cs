using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP3.Controllers;
using TP3.Data;
using TP3.Models;

namespace TestTP3;

public class BooksControllerTests
{
    private async Task<ApplicationDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        
        context.Books.Add(new Book { Id = 1, Title = "Test Book 1", Author = "Test Author 1" });
        context.Books.Add(new Book { Id = 2, Title = "Test Book 2", Author = "Test Author 2" });
        await context.SaveChangesAsync();
        
        return context;
    }

    [Fact]
    public async Task Index_ReturnsViewWithBooks()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Index() as ViewResult;

        Assert.NotNull(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Book>>(result.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_ReturnsNotFoundWhenIdIsNull()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Details(null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsNotFoundWhenBookDoesNotExist()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsViewResultWithBook()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Details(1) as ViewResult;

        Assert.NotNull(result);
        var book = Assert.IsType<Book>(result.Model);
        Assert.Equal(1, book.Id);
        Assert.Equal("Test Book 1", book.Title);
    }

    [Fact]
    public async Task Create_AddsNewBookAndRedirectsToIndex()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);
        var newBook = new Book { Title = "New Book", Author = "New Author" };

        var result = await controller.Create(newBook) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        var book = await context.Books.FirstOrDefaultAsync(b => b.Title == "New Book");
        Assert.NotNull(book);
        Assert.Equal("New Author", book.Author);
    }

    [Fact]
    public async Task Edit_ReturnsNotFoundWhenIdIsNull()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Edit(null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_ReturnsNotFoundWhenBookDoesNotExist()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_UpdatesBookAndRedirectsToIndex()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);
        var book = await context.Books.FindAsync(1);
        if (book != null)
        {
            book.Title = "Updated Title";
            book.Author = "Updated Author";

            var result = await controller.Edit(1, book) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        var updatedBook = await context.Books.FindAsync(1);
        Assert.Equal("Updated Title", updatedBook?.Title);
        Assert.Equal("Updated Author", updatedBook?.Author);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundWhenIdIsNull()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Delete(null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundWhenBookDoesNotExist()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesBookAndRedirectsToIndex()
    {
        var context = await GetDatabaseContext();
        var controller = new BooksController(context);

        var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        var deletedBook = await context.Books.FindAsync(1);
        Assert.Null(deletedBook);
    }
}