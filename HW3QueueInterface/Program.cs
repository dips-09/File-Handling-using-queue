using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW3QueueInterface
{
    class Program
    {
		private static void printUsage()
		{
			Console.WriteLine("Usage is:\n" +
				"\tC# Main C inputfile outputfile\n\n" +
				"Where:" +
				"  C is the column number to fit to\n" +
				"  inputfile is the input text file \n" +
				"  outputfile is the new output file base name containing the wrapped text.\n");
		}
		static void Main(string[] args)
        {
			
			int C = 72;                     // Column length to wrap to
			string inputFilename;
			string outputFilename = "output.txt";
			//StreamWriter sw= null;
			StreamReader sr = null;

			if (args.Length != 3)
			{
				printUsage();
				Environment.Exit(1);
			}

			try
			{
				
				C = Int32.Parse(args[0]);
				inputFilename = args[1];
				outputFilename = args[2];
				sr = new StreamReader(inputFilename);
			}
			catch (FileNotFoundException e)
			{
				
				Console.WriteLine("Could not find the input file.");
				Environment.Exit(1);
			}
			catch (Exception e)
			{
				Console.WriteLine("Something is wrong with the input.");
				printUsage();
				Environment.Exit(1);
			}
            /*finally
            {
				Console.WriteLine("Have a good day!");
				Environment.Exit(1);
            }*/

			IQueueInterface<string> words = new LinkedQueue<string>();

			// Read input file, tokenize by whitespace
			string str = sr.ReadLine();
			while (str != null)
			{
				string[] inpWords = str.Split(' ');
				foreach(string item in inpWords)
                {
					words.enqueue(item);
				}
				str = sr.ReadLine();
				
			}
			sr.Close();

			// At this point the input text file has now been placed, word by word, into a FIFO queue
			// Each word does not have whitespaces included, but does have punctuation, numbers, etc.

			/* ------------------ Start here ---------------------- */

			// As an example, do a simple wrap
			int spacesRemaining = WrapSimply(words, C, outputFilename);
			Console.WriteLine("Total spaces remaining (Greedy): " + spacesRemaining);
			Console.Read();

		}



		public static int WrapSimply(IQueueInterface<String> words, int columnLength, String outputFilename)
		{
            StreamWriter _out; 

			try
			{
				_out = new StreamWriter(outputFilename);
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("Cannot create or open " + outputFilename +
							" for writing.  Using standard output instead.");
                _out = new StreamWriter(Stream.Null);
			}

			int col = 1;
			int spacesRemaining = 0;
            // Running count of spaces left at the end of lines

            try
            {
				while (!words.isEmpty())
				{
					String str = words.peek();
					int len = str.Length;
					// See if we need to wrap to the next line
					if (col == 1)
					{
						_out.Write(str);
						col += len;
						words.dequeue();
					}
					else if ((col + len) >= columnLength)
					{
						// go to the next line
						_out.WriteLine();
						spacesRemaining += (columnLength - col) + 1;
						col = 1;
					}
					else
					{   // Typical case of printing the next word on the same line
						_out.Write(" ");
						_out.Write(str);
						col += (len + 1);
						words.dequeue();
					}

				}
			}
			catch(QueueUnderflowException e)
            {
				Console.WriteLine("Exception occured : The queue was empty");
            }
            finally
            {
				_out.WriteLine();
				_out.Flush();
				_out.Close();
				
			}
			return spacesRemaining;

		}
	}
}
