**SplitandMerge.exe**  
  
Tool to split/reassemble/run a file into/from chuncks to avoid signatures that may trigger on copy or execution.  
  
Includes the ability to merge and resize the chunks back into an exe, execute, delete, or run a merged .NET assembly entirely in memory.  
  
```
.\SplitandMerge.exe --help  
SplitandMerge 1.0.0.0  
Copyright c 2024  
  
  split      Split file into chunks  
             Output file chunks will include an index number.  
    
  merge      Merge file chunks into a memory stream  
             Optionally inflate the memory stream size  
             Optionally execute the memory stream if it is a .NET assembly  
             Optionally write inflated file to disk  
             Optionally run the file from disk instead of from memory  
             Optionally delete the file on disk  
    
  help       Display more information on a specific command.  
    
  version    Display version information.  
```
  
```
.\SplitandMerge.exe split --help  
SplitandMerge 1.0.0.0  
Copyright c 2024  
    
  -f, --file       Required. File to split into chunks  
    
  -c, --chunks     (Default: 4) Set number of equal chunks to split the file into  
    
  -p, --pattern    (Default: ?????#chunk??.dat) Set file pattern for the chunk files  
                   ?s are replaced with random alpha characters  
                   # is the location of the index number used for reconstruction ordering  
    
  -o, --out        (Default: .) Set output path  
    
  --help           Display this help screen.  
    
  --version        Display version information.  
```
  
```
.\SplitandMerge.exe merge --help  
SplitandMerge 1.0.0.0  
Copyright c 2024  
  
  -i, --inpath           (Default: .) Set path of files to merge  
  
  -p, --pattern          (Default: *chunk*) Set pattern to match for input files  
  
  -r, --resize           Optional. Resize\inflate the memorystream  
  
  -s, --size             (Default: 140) Pad memoystream until over this size in MB  
  
  -m, --memoryexecute    Optional. Execute the loaded assembly in memory  
  
  -w, --write            Optional. Write file to disk  
  
  -o, --out              (Default: .\Payload.exe) Optional. Set output file path and name  
  
  -e, --execute          Optional. Execute the written file with Process.Start  
  
  -d, --delete           Optional. Delete the written file afterward  
  
  --help                 Display this help screen.  
  
  --version              Display version information.  
```