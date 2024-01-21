# Vanity_Dump
Usage: SmartPIDSelection.exe [process name]  

C# tool/function Smart-ish-ly choose a named process PID based on the running Session, User Identity, and lowest PID number.  

For example, if running the executable as "NT AUTHORITY/SYSTEM" in Session 0, this funciton will be able to identify *only* "SYSTEM" svchost processes and select the lowest PID value.  

Potentially handy for things like process identification for PPID spoofing.  
