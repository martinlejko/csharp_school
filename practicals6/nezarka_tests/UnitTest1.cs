using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;

public class UnitTest1
{
    [Fact]
    public void TestInvalidOutput()
    {
        string htmlContent = "<!DOCTYPE html>\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n\t<meta charset=\"utf-8\" />\n\t<title>Nezarka.net: Online Shopping for Books</title>\n</head>\n<body>\n<p>Invalid request.</p>\n</body>\n</html>\n====";
        var writer = new StringWriter();
        Console.SetOut(writer);
        ModelStore bookstoreModel;
		View bookstoreView;
		Controller controller;
        bookstoreModel = new ModelStore();
        controller = new Controller(bookstoreModel);

        var reader = new StreamReader("data.txt");
        bookstoreView = new View(bookstoreModel, controller, reader);
        bookstoreView.CallInvalidRequest();
        var output = writer.GetStringBuilder().ToString().Trim();
        Assert.Equal(htmlContent, output);
       
    }

    [Fact]
    public void TestHeaderOutput(){
        string htmlContent = "<!DOCTYPE html>\n" +
        "<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\n" +
        "<head>\n" +
        "\t<meta charset=\"utf-8\" />\n" +
        "\t<title>Nezarka.net: Online Shopping for Books</title>\n" +
        "</head>\n" +
        "<body>\n" +
        "\t<style type=\"text/css\">\n" +
        "\t\ttable, th, td {\n" +
        "\t\t\tborder: 1px solid black;\n" +
        "\t\t\tborder-collapse: collapse;\n" +
        "\t\t}\n" +
        "\t\ttable {\n" +
        "\t\t\tmargin-bottom: 10px;\n" +
        "\t\t}\n" +
        "\t\tpre {\n" +
        "\t\t\tline-height: 70%;\n" +
        "\t\t}\n" +
        "\t</style>\n" +
        "\t<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>\n" +
        "\t" + "John"+ ", here is your menu:\n" +
        "\t<table>\n" +
        "\t\t<tr>\n" +
        "\t\t\t<td><a href=\"/Books\">Books</a></td>\n" +
        "\t\t\t<td><a href=\"/ShoppingCart\">Cart (" + "0" + ")</a></td>\n" +
        "\t\t</tr>\n" +
        "\t</table>";
        var writer = new StringWriter();
        Console.SetOut(writer);
        ModelStore bookstoreModel;
        View bookstoreView;
        Controller controller;
        bookstoreModel = new ModelStore();
        bookstoreModel.customers.Add(new Customer {Id = 1, FirstName = "John", LastName = "Doe"});
        int customerId = 1;
        controller = new Controller(bookstoreModel);
        bookstoreView = new View(bookstoreModel, controller, new StreamReader("data.txt"));
        bookstoreView.CallHeader(customerId);
        var output = writer.GetStringBuilder().ToString().Trim(); 
        Assert.Equal(htmlContent, output);
    }

    [Fact]
    public void Process_ValidRequest_ReturnsExpectedResult()
    {
        // Arrange
        string[] lines = new string[]
        {
            "GET 123 https://example.com/customers/123/orders",
            // Add more test cases here if needed
        };
        string htmlContent = "<!DOCTYPE html>\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n\t<meta charset=\"utf-8\" />\n\t<title>Nezarka.net: Online Shopping for Books</title>\n</head>\n<body>\n<p>Invalid request.</p>\n</body>\n</html>\n====";
        // Set up a StringReader to simulate the behavior of the reader object
        using (StringReader reader = new StringReader(string.Join(Environment.NewLine, lines)))
        {
            var writer = new StringWriter();
            Console.SetOut(writer);
            ModelStore bookstoreModel = new ModelStore();
            Controller controller = new Controller(bookstoreModel);
            View bookstoreView = new View(bookstoreModel, controller, reader);

            var output = writer.GetStringBuilder().ToString().Trim();
            // Act
            bookstoreView.Process();

        
            Assert.Equal(htmlContent, output);
        }
    }



}