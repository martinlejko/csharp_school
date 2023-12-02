using System;
using System.IO;

namespace CollumnSumTest {

    public class UnitTest1
    {
        //First two tets are the same as in the example in the assigment, after that are my own 10 tests
        [Fact]
        public void Recodex1()
        {
            // Arrange
            var testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    trzba
            leden   brambory    tuzemske    Bartak      10895       12      130740
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal(52, processor.sum);
            Assert.Equal("", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void Recodex2()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    trzba
            leden   brambory    tuzemske    Bartak      10895       12      130740
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("prodejce");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal(0, processor.sum);
            Assert.Equal("Invalid Integer Value\n", capturedOutput);

            writer.Close();
        }

        //start of my own tests, also we do not assert the sum of the collumns when we know the test should output an error becauase the sum is just a partial value that would not written
        [Fact]
        public void FirstLineEmpty()
        {
            // Arrange
            string testInput = """

            mesic   zbozi       typ         prodejce    mnozstvi    cena    trzba
            leden   brambory    tuzemske    Bartak      10895       12      130740
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid File Format\n", capturedOutput);

            writer.Close();
        }



        [Fact]
        public void OnlyHeaderTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal(0, processor.sum);
            Assert.Equal("", capturedOutput);

            writer.Close();
        }
        [Fact]
        public void TabsRepresentedAsWhiteSpacesTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    trzba
            leden   brambory                tuzemske    Bartak      10895       12      130740
            leden                 brambory    vlastni     Celestyn    15478       10      154780
            leden                 jablka      dovoz                          Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal(52, processor.sum);
            Assert.Equal("", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void NumbersInsideOfWordsInHeaderTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    ce35n2a    cena
            leden   brambory    tuzemske    Bartak      10895       12      130740
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("ce35n2a");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal(52, processor.sum);
            Assert.Equal("", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void EmptyLinesInsideTableTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            leden   brambory    tuzemske    Bartak      10895       12      130740

            leden   brambory    vlastni     Celestyn    15478       10      154780


            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid File Format\n", capturedOutput);

            writer.Close();
        }
        [Fact]
        public void EmptyLineWithWhiteSpacesInsideTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            leden   brambory    tuzemske    Bartak      10895       12      130740
                                                         
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert 
            Assert.Equal("Invalid File Format\n", capturedOutput);

            writer.Close();
        }


        [Fact]
        public void NotTheSameNumberOfWords()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            leden   brambory    tuzemske    Bartak      10895       12      
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid File Format\n", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void FirstLineOnlyWhiteSpacesTest()
        {
            // Arrange
            string testInput = """

            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            leden   brambory    tuzemske    Bartak      10895       12      130740
            leden   brambory    vlastni     Celestyn    15478       10      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid File Format\n", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void EmptyFileTest()
        {
            // Arrange
            string testInput = "";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid File Format\n", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void NotAllValuesAreIntTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            leden   brambory    tuzemske    Bartak      10895       12      130740
            leden   brambory    vlastni     Celestyn    15478       jozo      154780
            leden   jablka      dovoz       Adamec      1321        30      39630
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid Integer Value\n", capturedOutput);

            writer.Close();
        }

        [Fact]
        public void MoreCollumnsWithTheSameNameTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    cena
            leden   brambory    tuzemske    Bartak      10895       12      130
            leden   brambory    vlastni     Celestyn    15478       10      15478
            leden   jablka      dovoz       Adamec      1321        30      39
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal(52, processor.sum);
            Assert.Equal("", capturedOutput);

            writer.Close();
        }


        [Fact]
        public void NonExistentCollumnNameTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    trzba
            leden   brambory    tuzemske    Bartak      10895       12      130
            leden   brambory    vlastni     Celestyn    15478       10      15478
            leden   jablka      dovoz       Adamec      1321        30      39
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("kori");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Non-existent Column Name\n", capturedOutput);

            writer.Close();
        }


        [Fact]
        public void BigNumbersTest()
        {
            // Arrange
            string testInput = """
            mesic   zbozi       typ         prodejce    mnozstvi    cena    trzba
            leden   brambory    tuzemske    Bartak      10895       12192357513451      130
            leden   brambory    vlastni     Celestyn    15478       101543215123532      15478
            leden   jablka      dovoz       Adamec      1321        303152512413241234      39
            """;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testInput);
            writer.Flush();
            stream.Position = 0;
            var processor = new LineProcessor(new StreamReader(stream), writer);


            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            processor.Process("cena");
            string capturedOutput = consoleOutput.ToString();
            // Assert
            Assert.Equal("Invalid Integer Value\n", capturedOutput);

            writer.Close();
        }
    }
}
