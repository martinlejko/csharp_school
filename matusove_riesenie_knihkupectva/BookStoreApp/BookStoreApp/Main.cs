using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace NezarkaBookstore
{
    internal class Program
    {        
        public static void Main()
        {
            try
            {
                StreamReader reader = new StreamReader("data.txt");
                ModelStore store = ModelStore.LoadFrom(reader);
                Processor controller = new Processor(store);
                
                HtmlGenerator htmlGenerator = new HtmlGenerator(store, controller,reader);
                htmlGenerator.ProcessRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Data error: " + ex.Message);
            }
        }

    }
}
