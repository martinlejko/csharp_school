using System;
using System.Data;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace TestWordCount;

public class WordCounterTests
{
    [Fact]
    public void readParagraphsTest()
    {
        Program program = new Program();
        var reader = new StringReader("If a train station is where the train stops,\nwhat is a work station?");
        program.Main();

    }

    // [Fact]
    // public void PrintLineNoWordsNoHighlight_Test()
    // {
    //     // Arrange
    //     const int lineLength = 12;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, false);
    //     wordCounter.words = new List<string> {};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineNoWordsHighlight_Test()
    // {
    //     // Arrange
    //     const int lineLength = 12;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, true);
    //     wordCounter.words = new List<string> {};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "<-\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineOneWordNoHightLight_Test()
    // {
    //     // Arrange
    //     const int lineLength = 12;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, false);
    //     wordCounter.words = new List<string> {"word"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "word\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineOneWordHightLight_Test()
    // {
    //     // Arrange
    //     const int lineLength = 12;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, true);
    //     wordCounter.words = new List<string> {"word"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength =  wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "word<-\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineTwoWordsNoHightLightOneSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 9;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, false);
    //     wordCounter.words = new List<string> {"word", "word"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "word word\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineTwoWordsHightLightOneSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 9;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, true);
    //     wordCounter.words = new List<string> {"word", "word"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "word.word<-\n";
    //     Assert.Equal(expectedOutput, output);
    // }


    // [Fact]
    // public void PrintLineTwoWordsNoHightLightEvenNumberOfSpacesInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 10;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, false);
    //     wordCounter.words = new List<string> {"word", "word"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "word  word\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineTwoWordsHightLightEvenNumberOfSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 10;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, true);
    //     wordCounter.words = new List<string> {"word", "word"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "word..word<-\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    //     [Fact]
    // public void PrintLineMultipleWordsNoHightLightEvenNumberSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 25;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, false);
    //     wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "Martin  Lejko  CVUT  code\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineMultipleWordsHightLightEvenNumberSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 25;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, true);
    //     wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "Martin..Lejko..CVUT..code<-\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    //         [Fact]
    // public void PrintLineMultipleWordsNoHightLightOddNumberSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 23;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, false);
    //     wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "Martin  Lejko CVUT code\n";
    //     Assert.Equal(expectedOutput, output);
    // }

    // [Fact]
    // public void PrintLineMultipleWordsHightLightOddNumberSpaceInBetween_Test()
    // {
    //     // Arrange
    //     const int lineLength = 23;
    //     var input = "";
    //     var reader = new StringReader(input);
    //     var writer = new StringWriter();
    //     var wordCounter = new WordCounter(reader, writer, lineLength, true);
    //     wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
    //     wordCounter.wordsCount = wordCounter.words.Count;
    //     wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
    //     // Act
    //     wordCounter.printLine();
    //     var output = writer.ToString();

    //     // Assert
    //     string expectedOutput = "Martin..Lejko.CVUT.code<-\n";
    //     Assert.Equal(expectedOutput, output);
    // }
}










// // EXPLANATION OF THE TESTS
// // TIETO TESTY SU INTEGRACNE Z MINULA ALE TAKTIEZ BOLI POUZITE PRI PISANI KODU

// public class WordCounterTests2
// {
//     private void RunWordCounterTest(string input, string expectedOutput, int maxParagraphLength = 0, bool hihglightSpaces = false) {
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var counter = new WordCounter(reader, writer, maxParagraphLength, hihglightSpaces);
//         counter.readParagraphs();
//         counter.processLine(new Token { Type = Token.TokenType.EndOfParagraph });
//         string actualOutput = writer.ToString();
//         Assert.Equal(expectedOutput, actualOutput);
//     }



//     [Fact]
//     public void RecodexTest1() {

//         string input = "If a train station is where the train stops,\nwhat is a work station?";
//         string expectedOutput = "If     a    train\nstation  is where\nthe  train stops,\nwhat  is  a  work\nstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }
//         [Fact]
//     public void EmptyFileTest() {

//         string input = "";
//         string expectedOutput = "";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//     [Fact]
//     public void OnlyNewLinesTest() {

//         string input = """
        


//         """;
//         string expectedOutput = "";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//     [Fact]
//     public void WhiteSpacesLineTest() {

//         string input = """

                                 
//         """;
//         string expectedOutput = "";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//     [Fact]
//     public void BasicOneLine() {

//         string input = """
//         If a train station is where the train stops.
//         """;
//         string expectedOutput = "If a train\nstation is\nwhere  the\ntrain\nstops.\n";
//         RunWordCounterTest(input, expectedOutput, 10);
//     }

//     [Fact]
//     public void WordLongerThenRange() {
//         string input = """
//         dsibagiasdbgibasidgbiabg aigbsaig abgisbg
//         """;
//         string expectedOutput = "dsibagiasdbgibasidgbiabg\naigbsaig\nabgisbg\n";
//         RunWordCounterTest(input, expectedOutput, 5);
//     }

//     [Fact]
//     public void TwoNormalParagraphsTest() {

//         string input = """
//         If a train station is where the train stops.
        
//         what is a workstation?
//         """;
//         string expectedOutput = "If     a    train\nstation  is where\nthe train stops.\n\nwhat     is     a\nworkstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }

//     [Fact]
//     public void LeadingAndMultipleNewLinesTest() {

//         string input = """




//         If a train station is where the train stops.
        



//         what is a workstation?



//         """;
//         string expectedOutput = "If     a    train\nstation  is where\nthe train stops.\n\nwhat     is     a\nworkstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//    [Fact]
//     public void PerfectFittingTest() {

//         string input = """
//         If a train
//         station is
//         where thed
//         train stop
        
//         what is ae
//         workstatio
//         """;
//         string expectedOutput = "If a train\nstation is\nwhere thed\ntrain stop\n\nwhat is ae\nworkstatio\n";
//         RunWordCounterTest(input, expectedOutput, 10);

//     }

//     [Fact]
//     public void TabsAsWhiteSpacesTest() {

//         string input = """
//         If a train              station is where the train stops.
        
//         what is a                work station?
//         """;
//         string expectedOutput = "If     a    train\nstation  is where\nthe train stops.\n\nwhat  is  a  work\nstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }

//     [Fact]
//     public void NumbersAsNonWhiteTest() {

//         string input = """
//         If a 12345 station 12 where 123 train stops.
//         """;
//         string expectedOutput = "If     a    12345\nstation  12 where\n123 train stops.\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }
// }
