using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.IO.Compression;

/*
 * Alternative for filestream.copyto() 
 * http://social.msdn.microsoft.com/Forums/en-US/csharplanguage/thread/6225a6bd-8b2d-45ac-8d0e-86409121e196
 */
 
public class Zip : MonoBehaviour {
	
	bool zipflag = true;
	
	string directoryPath = string.Empty;
	
	// Use this for initialization
	void Start () 
	{     
		directoryPath = Application.dataPath + @"/Data/Log/";
		
//            foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.gz"))
//            {
//                Decompress(fileToDecompress);
//            }
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if(MainGuiControls.endXml && zipflag)
		{
			
			DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);
            foreach (FileInfo fileToCompress in directorySelected.GetFiles())
            {
                Compress(fileToCompress);
            }
			
			zipflag = false;
		}
	}
	
	
	public static void CopyStream(Stream input, Stream output)
	{
    byte[] buffer = new byte[32768];
    int read;

    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
    	{
        output.Write (buffer, 0, read);
    	}
	}
	
	
	public static void Compress(FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
							
							CopyStream(originalFileStream,compressionStream);
                        //    originalFileStream.CopyTo(compressionStream);
//                            Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
//                                fileToCompress.Name, fileToCompress.Length.ToString(), compressedFileStream.Length.ToString());
							Debug.Log("Compressed: "+fileToCompress.Name+" from "+ fileToCompress.Length.ToString() +" to "+ compressedFileStream.Length.ToString()+" bytes.");
                        }
                    }
                }
            }
        }
	
	
//        public static void Decompress(FileInfo fileToDecompress)
//        {
//            using (FileStream originalFileStream = fileToDecompress.OpenRead())
//            {
//                string currentFileName = fileToDecompress.FullName;
//                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
//
//                using (FileStream decompressedFileStream = File.Create(newFileName))
//                {
//                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
//                    {
//                  //      decompressionStream.CopyTo(decompressedFileStream);
//                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
//                    }
//                }
//            }
//		}
	
	
}
