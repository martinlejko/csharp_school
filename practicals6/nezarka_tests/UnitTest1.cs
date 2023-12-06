using System;
using System.IO;
using Xunit;

public class UnitTest1
{
    [Fact]
    public void TestConsoleInputOutput()
    {
        // Arrange
        string testData = "DATA-BEGIN\nBOOK;1;Book Title;Author;25.99\nDATA-END";
        File.WriteAllText("data.txt", testData);

        string expectedOutput = "EXPECTED OUTPUT";

        using (StringWriter sw = new StringWriter())
        {
            using (StringReader sr = new StringReader(testData))
            {
                Console.SetOut(sw);
                Console.SetIn(sr);

                // Act
                Porgram.Main();

                // Assert
            string actualOutput = sw.ToString().Trim();
            Assert.Equal(expectedOutput, actualOutput);
        }
    }
}
}