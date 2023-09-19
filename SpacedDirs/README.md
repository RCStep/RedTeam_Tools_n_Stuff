# SpacedDirs
Usage: SpacedDirs.exe [create/delete] [directory]  

C# tool to create/delete Directories that forces acceptance of trailing blank spaces.  

Wrap the directory path including spaces in quotes.  
SpacedDirs.exe create "C:\windows " then run SpacedDirs.exe create "C:\windows \system32\"  
SpacedDirs.exe delete "C:\windows \system32" then run SpacedDirs.exe delete "C:\windows "  
Can only create/delete the end-most directory at a time. The directory must be empty and not occupied/locked to delete.  