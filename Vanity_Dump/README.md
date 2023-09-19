# Vanity_Dump
Usage: Vanity_Dump.exe [target PID] [output_file_path]  

C++ tool that creates a reflected copy of a target process using RtlCreateProcessReflection and then performs a minidumpwirtedump on this copy.  

Potentially handy for things like LSASS but also browsers and password managers that may be harboring secrets.  

Must be elevated. This tool will also attempt to add SeDebugPrivilege.  
