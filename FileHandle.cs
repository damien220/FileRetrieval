using System.Diagnostics;
using System;

namespace FileRetrieval
{
    public class FileHandle
    {

        public void SplitFile(string filePath)
        {
            //Check if we are handling a real file
            if (filePath == null || Directory.Exists(filePath) || !System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File path cannot be empty or a directory!");
                return;
            }
            Console.WriteLine("Begin split.......");
            AvailableObjectsData AvailableObj = new AvailableObjectsData();
            try
            {
                using (var inputFile = System.IO.File.OpenRead(filePath))
                {
                    //write the name
                    string FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                    AvailableObj.Name = FileName;
                    //Console.WriteLine("File name is:{0}", AvailableObj.Name);
                    //write the size
                    long size = inputFile.Length;
                    //Console.WriteLine("File size:{0}",size);
                    AvailableObj.size = size;
                    int numParts = size < 250 * 1000000 ? 8 : 10;
                    long chunkSize = size / numParts;
                    //Console.WriteLine($"Chunk size: {chunkSize} - with {numParts} parts");

                    byte[] buffer = new byte[chunkSize];
                    string[] Parts = new string[numParts];

                    for (int i = 0; i < numParts; i++)
                    {
                        string partFilePath = i % 2 == 0 ? Path.Combine(PairPartFiles, $"{AvailableObj.Name}.Part-{i + 1}") :
                                                        Path.Combine(OddPartFiles, $"{AvailableObj.Name}.Part-{i + 1}");
                        Parts[i] = partFilePath;
                        using (var outputFile = System.IO.File.Create(partFilePath))
                        {
                            //Console.WriteLine($"Writing to file: {partFilePath}");

                            if (i == numParts - 1)
                            {
                                // last chunk may be larger than the others
                                long remainingBytes = size - (i * chunkSize);
                                buffer = new byte[remainingBytes];
                            }
                            inputFile.Read(buffer, 0, buffer.Length);
                            outputFile.Write(buffer, 0, buffer.Length);
                        }
                    }
                    AvailableObj.PartsPaths = Parts;
                    Console.WriteLine("Split completed");
                }
                rootObject.availableObjects.Add(AvailableObj);
                WriteJson(AvailableObj);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error creating file: " + ex.Message);
            }
        }


        public void JoinFiles(int num)
        {
            Console.WriteLine("Begin join");
            try
            {
                string property = "Id";
                string fileContent = System.IO.File.ReadAllText(AvailableFiles);
                //Parse the json file as array
                JArray jsonArray = JArray.Parse(fileContent);
                //Get the entry having the correct id num
                IEnumerable<JToken> targetObjects = jsonArray.Where(obj => obj[property].Value<int>() == num);
                // Extract the values of the parts property from the target objects
                IEnumerable<JToken> partsValues = targetObjects.SelectMany(o => o["PartsPaths"]);
                string[] partsArray = partsValues.Select(p => (string)p).ToArray();
                //Console.WriteLine("parts arrray: " + partsArray.Count());

                using (FileStream fileStream = new FileStream(@current, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    foreach (string partFilePath in partsArray)
                    {
                        Console.WriteLine($"Reading from file: {partFilePath}");

                        using (var inputFile = System.IO.File.OpenRead(partFilePath))
                        {
                            fileStream.CopyTo(inputFile);
                            //inputFile.CopyTo(outputFile);
                        }

                        System.IO.File.Delete(partFilePath);
                    }
                    Console.WriteLine("Join completed");
                    Process.Start(current);
                }
            }

            catch (Newtonsoft.Json.JsonException j)
            {
                Console.WriteLine(j.ToString());
            }

            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Error creating file: " + ex.Message);
            }
            /*      using (var outputFile = System.IO.File.Create(@current))
                {
                    foreach (string partFilePath in partsArray)
                    {
                        Console.WriteLine($"Reading from file: {partFilePath}");

                        using (var inputFile = System.IO.File.OpenRead(partFilePath))
                        {
                            inputFile.CopyTo(outputFile);
                        }

                        System.IO.File.Delete(partFilePath);
                    }
                    Console.WriteLine("Join completed");
                    Process.Start(current);

                }*/
        }


    }
}
