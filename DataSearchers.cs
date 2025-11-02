namespace JamDataFinder;

public abstract class DataSearchers
{
    public static void FindHd(string inFile, string outDir)
    {
        var simpleName = Path.GetFileNameWithoutExtension(inFile);
        var outNameFormat = Path.Combine(outDir, simpleName + ".{0}.HD");
        using var inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
        var id = 0;
        Console.WriteLine("Searching for JAM headers");
        Console.Write("Please wait...");
        for (var os = 0; os < inStream.Length - 4; os++)
        {
            inStream.Position = os;
            var buffer = new byte[4];
            inStream.ReadExactly(buffer, 0, buffer.Length);
            // verify that the SShd header is present at offset 0x0C
            if (BitConverter.ToInt32(buffer) != 0x64685353) continue;

            inStream.Position -= 8;
            inStream.ReadExactly(buffer, 0, buffer.Length);
            if (BitConverter.ToInt32(buffer) != 0) // verify that bytes 0x08 to 0x0B are 0 
            {
                inStream.Position += 4;
                continue;
            }

            inStream.Position -= 0xC; // back to beginning of header
            inStream.ReadExactly(buffer, 0, buffer.Length);
            var length = BitConverter.ToInt32(buffer, 0); // length of header

            inStream.Position -= 4; // back to beginning of header
            
            
            var hdData = new byte[length];
            if (inStream.Position + length > inStream.Length) continue;
            Console.Write($"\r{++id} header(s) found so far");
            inStream.ReadExactly(hdData, 0, length);
            File.WriteAllBytes(string.Format(outNameFormat, id), hdData);
        }
        inStream.Close();
        if (id == 0)
        {
            Console.WriteLine("\rNo headers found!");
            return;
        }
        Console.WriteLine("\nFinished!");
    }
    
    public static void FindBd(string inFile, string hdFile, string outDir)
    {
        Console.WriteLine("Identifying parameters for finding a matching BD file");
        var simpleName = Path.GetFileNameWithoutExtension(inFile);
        var outPath = Path.Combine(outDir, simpleName + ".{0}.BD");
        using var hdStream = new FileStream(hdFile, FileMode.Open, FileAccess.Read);
        hdStream.Position = 4;
        var buffer = new byte[4];
        var id = 0;
        hdStream.ReadExactly(buffer, 0, buffer.Length);
        var bdLength = BitConverter.ToInt32(buffer, 0);
        hdStream.Position = 0x10;
        hdStream.ReadExactly(buffer, 0, buffer.Length);
        var programChunkOffset = BitConverter.ToInt32(buffer, 0);
        if (programChunkOffset == -1)
        {
            Console.WriteLine("Error: JAM body search is not supported for JAM header files that don't have a program chunk");
            return;
        }
        hdStream.Position = programChunkOffset;
        buffer = new byte[2];
        hdStream.ReadExactly(buffer, 0, buffer.Length);
        var entryCount = BitConverter.ToInt16(buffer, 0);
        var relativeOffset = hdStream.Position - 2;
        var offsets = new short[entryCount + 1];
        for (var i = 0; i <= entryCount; i++) // prog chunk offsets
        {
            hdStream.ReadExactly(buffer, 0, buffer.Length);
            offsets[i] = BitConverter.ToInt16(buffer, 0);
        }

        short minOffset = 0x7FFF;

        foreach (var offset in offsets) // prog chunk
        {
            if (offset == -1) continue;
            hdStream.Position = offset + relativeOffset;
            var count = (hdStream.ReadByte() & 0x7F) + 1;
            hdStream.Position = offset + relativeOffset + 8;
            for (var j = 0; j < count; j++) // split prog chunk
            {
                hdStream.Position += 0x4;
                hdStream.ReadExactly(buffer, 0, buffer.Length);
                var sampleOffset = BitConverter.ToInt16(buffer, 0);
                if (sampleOffset > 0 && sampleOffset < minOffset)
                {
                    minOffset = sampleOffset;
                }

                hdStream.Position += 0xA;
            }
        }
        hdStream.Close();
        
        Console.WriteLine("Start searching for VAG headers...");
        using var bdStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);

        for (var os = 0; os < bdStream.Length - 0x20; os++)
        {
            bdStream.Position = os;
            buffer = new byte[0x20];
            bdStream.ReadExactly(buffer, 0, buffer.Length);
            var sum16 = 0;
            for (var s = 0; s < buffer.Length / 2; s++)
            {
                sum16 += buffer[s];
            }

            if (sum16 != 0 || (buffer[16] == 0x00)) continue; // identify VAG header
            Console.Write($"\rFound a (potential) VAG header at 0x{os:X}");
            bdStream.Position = os + (minOffset * 8) - 0xE;
            var match = true;
            for (var k = 0; k < 0xE; k++)
            {
                if (bdStream.ReadByte() == 0x77) continue;
                match = false;
                break;
            }

            if (!match) continue;

            Console.WriteLine("\nFound a matching BD file! Saving...");
            id++;
            bdStream.Position = os;
            using var outStream = new FileStream(string.Format(outPath, id), FileMode.Create, FileAccess.Write);
            for (var i = 0; i < bdLength; i++)
            {
                outStream.WriteByte((byte)bdStream.ReadByte());
            }
            outStream.Close();
        }
        Console.WriteLine("Finished!");
    }
    
    public static void FindSeq(string inFile, string outDir)
    {
        var simpleName = Path.GetFileNameWithoutExtension(inFile);
        var outNameFormat = Path.Combine(outDir, simpleName + ".{0}.SQ");
        using var inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
        var id = 0;
        Console.WriteLine("Searching for sequence data");
        Console.Write("Please wait...");
        for (var os = 0; os < inStream.Length - 4; os++)
        {
            inStream.Position = os;
            var buffer = new byte[4];
            inStream.ReadExactly(buffer, 0, buffer.Length);
            // verify that the SShd header is present at offset 0x0C
            if (BitConverter.ToInt32(buffer) != 0x71735353) continue;
            
            inStream.Position = os - 4;
            var sum6 = 0;
            for (var s = 0; s < 4; s++)
            {
                sum6 += inStream.ReadByte();
            }
            if (sum6 != 0) continue; // verify that bytes 8 to 11 are 0
            
            // the header is considered valid
            Console.Write($"\r{++id} header(s) found so far");
            inStream.Position = os - 0xC;
            using var outStream = new FileStream(string.Format(outNameFormat, id),  FileMode.Create, FileAccess.Write);
            
            // header
            buffer = new byte[0x10];
            inStream.ReadExactly(buffer, 0, buffer.Length);
            outStream.Write(buffer, 0, buffer.Length);
            
            // channels
            buffer = new byte[0x10*0x10];
            inStream.ReadExactly(buffer, 0, buffer.Length);
            outStream.Write(buffer, 0, buffer.Length);
            
            // sequence
            var lastMessage = "\0\0"u8.ToArray();
            buffer = new byte[2];
            while (inStream.Position < inStream.Length - 2)
            {
                // keep writing until we encounter this sequence of bytes: 00 FF 2F 00
                inStream.ReadExactly(buffer, 0, buffer.Length);
                outStream.Write(buffer, 0, buffer.Length);
                if (buffer[0] == 0x2F && buffer[1] == 0x00 && lastMessage[0] == 0x00 && lastMessage[1] == 0xFF)
                {
                    break;
                }
                lastMessage[0] = buffer[0];
                lastMessage[1] = buffer[1];
            }
            outStream.Close();
        }
        inStream.Close();
        if (id == 0)
        {
            Console.WriteLine("\rNo headers found!");
            return;
        }
        Console.WriteLine("\nFinished!");
    }
}